// PROTOTYPE - throwaway.
import type { Film, Watch } from './mockData';

export interface VariantProps {
  watches: Watch[];
  /** Logs a Watch (date defaults to today). Returns an error message, or null when saved. */
  onLog: (film: Film, date?: string) => string | null;
  highlightId: number | null;
}
