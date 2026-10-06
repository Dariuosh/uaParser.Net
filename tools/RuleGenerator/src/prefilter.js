'use strict';

// Works out, for one regex, words that any match must contain, so the parser can skip the
// regex with a cheap substring check.
//
// The result is a list of groups: a match is only possible when, for every group, the input
// contains at least one of the group's words. Words are lowercase; the parser compares them
// with the lowercased input, and only for all-ASCII input, where that is exactly what a
// case-insensitive regex does. When in doubt a part contributes nothing, so the filter can only
// be too permissive, never too strict. rules.js checks this against the whole corpus.

// Small sets of possible strings are expanded (for example "-[ln]" becomes "-l" or "-n").
const MAX_STRINGS = 64;
const MAX_CLASS_CHARS = 10;

// --- A small parser for the JavaScript regex syntax ua-parser-js uses ----------------------

function parse(source) {
    let i = 0;

    function alternation() {
        const branches = [sequence()];
        while (source[i] === '|') {
            i++;
            branches.push(sequence());
        }
        return branches.length === 1 ? branches[0] : { type: 'alt', branches };
    }

    function sequence() {
        const items = [];
        while (i < source.length && source[i] !== '|' && source[i] !== ')') {
            items.push(quantified(parseAtom()));
        }
        return { type: 'seq', items };
    }

    function quantified(atom) {
        const ch = source[i];
        let min, max;
        if (ch === '*') { min = 0; max = Infinity; i++; }
        else if (ch === '+') { min = 1; max = Infinity; i++; }
        else if (ch === '?') { min = 0; max = 1; i++; }
        else if (ch === '{') {
            const m = /^\{(\d*)(,?)(\d*)\}/.exec(source.slice(i));
            if (!m || (m[1] === '' && m[3] === '')) return atom;   // a literal "{"
            min = m[1] === '' ? 0 : Number(m[1]);
            max = m[2] ? (m[3] === '' ? Infinity : Number(m[3])) : min;
            i += m[0].length;
        } else {
            return atom;
        }
        if (source[i] === '?') i++;                               // lazy
        return { type: 'repeat', atom, min, max };
    }

    function parseClass() {
        // [...]: the set of characters it accepts, or null when it is negated or large.
        let negated = false;
        if (source[i] === '^') { negated = true; i++; }
        const chars = new Set();
        let unknown = false, first = true;
        while (i < source.length && (source[i] !== ']' || first)) {
            first = false;
            let ch = source[i++];
            if (ch === '\\') {
                const e = source[i++];
                if (e === 'd') { for (let d = 0; d <= 9; d++) chars.add(String(d)); continue; }
                if (/[wWsSDbB]/.test(e)) { unknown = true; continue; }
                if (/[nrtvf0cxu]/.test(e)) throw new Error(`unsupported escape \\${e} in ${source}`);
                ch = e;
            }
            if (source[i] === '-' && source[i + 1] !== ']' && i + 1 < source.length) {
                let end = source[i + 1];
                i += 2;
                if (end === '\\') { end = source[i++]; if (/[a-zA-Z]/.test(end)) { unknown = true; continue; } }
                for (let c = ch.charCodeAt(0); c <= end.charCodeAt(0); c++) chars.add(String.fromCharCode(c));
                continue;
            }
            chars.add(ch);
        }
        if (source[i] !== ']') throw new Error(`missing "]" in ${source}`);
        i++;
        if (negated || unknown) return { type: 'other' };
        const lowered = new Set([...chars].map(c => c.toLowerCase()));
        return lowered.size <= MAX_CLASS_CHARS ? { type: 'set', strings: [...lowered] } : { type: 'other' };
    }

    function parseAtom() {
        const ch = source[i];
        if (ch === '(') {
            i++;
            let kind = 'group';
            if (source.startsWith('?:', i)) { i += 2; }
            else if (source.startsWith('?=', i) || source.startsWith('?!', i)) { kind = 'look'; i += 2; }
            else if (source.startsWith('?<=', i) || source.startsWith('?<!', i)) { kind = 'look'; i += 3; }
            else if (source[i] === '?') throw new Error(`unsupported group at ${i} in ${source}`);
            const body = alternation();
            if (source[i] !== ')') throw new Error(`missing ")" in ${source}`);
            i++;
            return { type: kind, body };
        }
        if (ch === '[') {
            i++;
            return parseClass();
        }
        if (ch === '\\') {
            const next = source[i + 1];
            i += 2;
            if (next === 'd') return { type: 'set', strings: '0123456789'.split('') };
            if (next === 'b' || next === 'B') return { type: 'empty' };
            if (/[DwWsS]/.test(next)) return { type: 'other' };
            if (/[1-9]/.test(next)) return { type: 'other' };                 // backreference
            if (/[nrtvf0cxu]/.test(next)) throw new Error(`unsupported escape \\${next} in ${source}`);
            return { type: 'set', strings: [next.toLowerCase()] };             // \/ \. \- \_ ...
        }
        i++;
        if (ch === '^' || ch === '$') return { type: 'empty' };
        if (ch === '.') return { type: 'other' };
        return { type: 'set', strings: [ch.toLowerCase()] };
    }

    const tree = alternation();
    if (i !== source.length) throw new Error(`unexpected "${source[i]}" at ${i} in ${source}`);
    return tree;
}

// --- Required words ---------------------------------------------------------------------------
//
// info(node) describes what any match of node must look like:
//   exact:  when not null, a small set that contains every string the node can match
//   groups: requirements (lists of alternative words) that every match satisfies

function product(a, b) {
    if (a.length * b.length > MAX_STRINGS) return null;
    const out = new Set();
    for (const x of a) for (const y of b) out.add(x + y);
    return [...out];
}

function info(node, choose) {
    switch (node.type) {
        case 'set':
            return { exact: node.strings, groups: [] };
        case 'empty':
        case 'look':                  // zero-width: it consumes nothing
            return { exact: [''], groups: [] };
        case 'other':
            return { exact: null, groups: [] };
        case 'group':
            return info(node.body, choose);
        case 'repeat': {
            const inner = info(node.atom, choose);
            if (node.max === 1 && node.min <= 1) {
                if (node.min === 1) return inner;
                return { exact: inner.exact ? [...new Set(['', ...inner.exact])] : null, groups: [] };
            }
            if (node.min === 0) return { exact: null, groups: [] };
            if (inner.exact && node.min === node.max) {
                let exact = [''];
                for (let n = 0; n < node.min && exact; n++) exact = product(exact, inner.exact);
                if (exact) return { exact, groups: [] };
            }
            // At least one copy of the atom is in every match.
            const words = inner.exact && !inner.exact.includes('') ? [inner.exact] : [];
            return { exact: null, groups: [...words, ...inner.groups] };
        }
        case 'seq': {
            // Glue neighbouring small sets into words; anything else ends the current run.
            // The node is only "exact" when one run covers it whole; once a run is cut, the
            // pieces are separate requirements and must not be glued to neighbouring nodes.
            const groups = [];
            let run = [''], whole = true;
            const flush = () => { if (!run.includes('')) groups.push(run); run = ['']; };
            for (const item of node.items) {
                const it = info(item, choose);
                if (it.exact) {
                    const joined = product(run, it.exact);
                    if (joined) { run = joined; continue; }
                    whole = false;
                    flush();
                    run = it.exact;
                    continue;
                }
                whole = false;
                flush();
                groups.push(...it.groups);
            }
            if (whole) return { exact: run, groups: [] };
            flush();
            return { exact: null, groups };
        }
        case 'alt': {
            const branches = node.branches.map(b => info(b, choose));
            if (branches.every(b => b.exact)) {
                const union = [...new Set(branches.flatMap(b => b.exact))];
                if (union.length <= MAX_STRINGS) return { exact: union, groups: [] };
            }
            // A match goes through one branch, so it satisfies one of that branch's requirements.
            const options = [];
            for (const b of branches) {
                const own = b.exact && !b.exact.includes('') ? [b.exact, ...b.groups] : b.groups;
                if (own.length === 0) return { exact: null, groups: [] };
                options.push(...choose(own));
            }
            return { exact: null, groups: [simplify(options)] };
        }
        default:
            throw new Error(`unknown node ${node.type}`);
    }
}

// Drops words that contain another word of the same group (containing the shorter one is
// already implied), and duplicates.
function simplify(words) {
    const unique = [...new Set(words)].sort((a, b) => a.length - b.length || (a < b ? -1 : 1));
    return unique.filter((w, n) => !unique.slice(0, n).some(shorter => w.includes(shorter)));
}

// The requirement that filters most: the one the fewest sample inputs satisfy (ties: fewer words).
function chooser(samples) {
    const cache = new Map();
    const hits = group => {
        const key = group.join('\u0000');
        if (!cache.has(key)) cache.set(key, samples.filter(s => group.some(w => s.includes(w))).length);
        return cache.get(key);
    };
    return groups => {
        let best = null;
        for (const g of groups.map(simplify)) {
            if (!best || hits(g) < hits(best) || (hits(g) === hits(best) && g.length < best.length)) best = g;
        }
        return best;
    };
}

// Requirements for a whole regex, or [] when nothing useful is known. samples: lowercased
// user agents used only to pick the most selective of several valid requirements.
function requiredWords(regex, samples = []) {
    const choose = chooser(samples);
    const it = info(parse(regex.source), choose);
    let groups = it.exact ? (it.exact.includes('') ? [] : [it.exact]) : it.groups;
    groups = groups.map(simplify).filter(g => g.length > 0 && g.every(w => /^[\x00-\x7f]+$/.test(w)));
    // Keep the most selective groups first so the parser can stop early.
    const hits = g => samples.filter(s => g.some(w => s.includes(w))).length;
    return groups.sort((a, b) => hits(a) - hits(b) || a.length - b.length).slice(0, 4);
}

module.exports = { parse, requiredWords };
