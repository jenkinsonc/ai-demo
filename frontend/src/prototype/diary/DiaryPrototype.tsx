// PROTOTYPE - throwaway, for "What the Diary looks like" (issue 5, map "Film tracker v1").
// Three variants of the Diary, switchable via ?variant=A|B|C, on the app's only route (/).
// Watches are in memory; the stub search logs a Watch dated today and highlights it.
import { useState } from 'react';
import { initialWatches, todayIso, type Film, type Watch } from './mockData';
import { PrototypeSwitcher } from '../PrototypeSwitcher';
import { VariantA } from './VariantA';
import { VariantB } from './VariantB';
import { VariantC } from './VariantC';

const variants = [
  { key: 'A', name: 'Journal: by month, text only' },
  { key: 'B', name: 'Poster wall: by year, modal per Film' },
  { key: 'C', name: 'Ledger: flat table, Film page' },
];

export function DiaryPrototype() {
  const [variant, setVariant] = useState(() => new URLSearchParams(location.search).get('variant') ?? 'A');
  const [watches, setWatches] = useState<Watch[]>(initialWatches);
  const [highlightId, setHighlightId] = useState<number | null>(null);

  const changeVariant = (key: string) => {
    const url = new URL(location.href);
    url.searchParams.set('variant', key);
    history.replaceState(null, '', url);
    setVariant(key);
  };

  const onLog = (film: Film, date: string = todayIso()): string | null => {
    if (!date) return 'Enter a date.';
    if (date > todayIso()) return 'The date cannot be in the future.';
    if (watches.some((w) => w.filmId === film.id && w.date === date)) {
      return `${film.title} already has a Watch on that date.`;
    }
    const id = Math.max(...watches.map((w) => w.id)) + 1;
    setWatches([...watches, { id, filmId: film.id, date }]);
    setHighlightId(id);
    setTimeout(() => setHighlightId((h) => (h === id ? null : h)), 4000);
    return null;
  };

  const props = { watches, onLog, highlightId };
  return (
    <>
      {variant === 'A' && <VariantA {...props} />}
      {variant === 'B' && <VariantB {...props} />}
      {variant === 'C' && <VariantC {...props} />}
      <PrototypeSwitcher variants={variants} current={variant} onChange={changeVariant} />
    </>
  );
}
