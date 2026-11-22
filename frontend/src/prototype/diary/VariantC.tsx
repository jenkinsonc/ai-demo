// PROTOTYPE - throwaway. Variant C "Ledger": one flat table newest first, no grouping, small thumb +
// year + director columns, rewatch shown as a Watch-number column. Per-Film view is its own page.
// Search lives in a sticky side panel.
import { useState } from 'react';
import { filmById, fmt, ordinal, watchNumber, watchesOfFilm, newestFirst, highlightStyle, type Film } from './mockData';
import { Poster, StubSearch } from './shared';
import type { VariantProps } from './types';

const cell: React.CSSProperties = { padding: '0.4rem 0.6rem', borderBottom: '1px solid var(--weather-card-border)', textAlign: 'left', verticalAlign: 'middle' };

export function VariantC({ watches, onLog, highlightId }: VariantProps) {
  const [film, setFilm] = useState<Film | null>(null);
  const sorted = [...watches].sort(newestFirst);

  const side = (
    <aside style={{ position: 'sticky', top: 16, alignSelf: 'start', width: 260, flex: 'none' }}>
      <h2 style={{ fontSize: '1rem', marginTop: 0 }}>Log a Watch</h2>
      <StubSearch onPick={(f) => { onLog(f); setFilm(null); }} />
      <p style={{ fontSize: '0.8rem', opacity: 0.7 }}>{watches.length} Watches · {new Set(watches.map((w) => w.filmId)).size} Films</p>
    </aside>
  );

  if (film) {
    const ws = watchesOfFilm(watches, film.id);
    return (
      <div style={{ display: 'flex', gap: '2rem', width: '100%' }}>
        {side}
        <div style={{ flex: 1 }}>
          <button type="button" onClick={() => setFilm(null)} style={{ background: 'none', border: 'none', color: 'inherit', padding: 0, textDecoration: 'underline' }}>← Diary</button>
          <div style={{ display: 'flex', gap: '1.5rem', marginTop: '1rem' }}>
            <Poster film={film} width={180} />
            <div>
              <h2 style={{ margin: 0, fontSize: '1.8rem' }}>{film.title}</h2>
              <div style={{ opacity: 0.7 }}>{film.year} · directed by {film.director}</div>
              <table style={{ borderCollapse: 'collapse', marginTop: '1rem' }}>
                <thead><tr><th style={cell}>#</th><th style={cell}>Date</th></tr></thead>
                <tbody>
                  {ws.map((w, i) => (
                    <tr key={w.id}><td style={cell}>{ws.length - i}</td><td style={cell}>{fmt(w.date, { weekday: 'short', day: 'numeric', month: 'short', year: 'numeric' })}</td></tr>
                  ))}
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div style={{ display: 'flex', gap: '2rem', width: '100%' }}>
      {side}
      <table style={{ flex: 1, borderCollapse: 'collapse', fontSize: '0.9rem' }}>
        <thead>
          <tr>
            <th style={cell}>Date</th><th style={cell} /><th style={cell}>Film</th><th style={cell}>Year</th><th style={cell}>Director</th><th style={cell}>Watch</th>
          </tr>
        </thead>
        <tbody>
          {sorted.map((w) => {
            const f = filmById(w.filmId);
            const n = watchNumber(watches, w);
            return (
              <tr key={w.id} style={highlightStyle(w.id === highlightId)}>
                <td style={{ ...cell, whiteSpace: 'nowrap', fontVariantNumeric: 'tabular-nums' }}>{fmt(w.date, { day: '2-digit', month: 'short', year: 'numeric' })}</td>
                <td style={cell}><Poster film={f} width={28} /></td>
                <td style={cell}>
                  <button type="button" onClick={() => setFilm(f)} style={{ background: 'none', border: 'none', color: 'inherit', padding: 0, textAlign: 'left', font: 'inherit', textDecoration: 'underline' }}>{f.title}</button>
                </td>
                <td style={cell}>{f.year}</td>
                <td style={{ ...cell, opacity: 0.8 }}>{f.director}</td>
                <td style={{ ...cell, opacity: n > 1 ? 1 : 0.5 }}>{n > 1 ? `${ordinal(n)} watch` : 'first'}</td>
              </tr>
            );
          })}
        </tbody>
      </table>
    </div>
  );
}
