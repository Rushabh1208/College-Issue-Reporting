import { useCallback, useEffect, useState } from "react";

export function useAsync(load, deps = [], options = {}) {
  const [data, setData] = useState(options.initialData ?? null);
  const [error, setError] = useState(null);
  const [isLoading, setIsLoading] = useState(Boolean(options.immediate ?? true));

  const run = useCallback(async () => {
    setIsLoading(true);
    setError(null);
    try {
      const result = await load();
      setData(result);
      return result;
    } catch (err) {
      setError(err);
      throw err;
    } finally {
      setIsLoading(false);
    }
  }, deps);

  useEffect(() => {
    if (options.immediate === false) return;
    run().catch(() => {});
  }, [run, options.immediate]);

  return { data, setData, error, isLoading, refetch: run };
}
