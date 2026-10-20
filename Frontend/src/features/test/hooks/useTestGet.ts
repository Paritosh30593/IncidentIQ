import { useQuery } from '@tanstack/react-query';
import { getTest } from '../api';

export const useTestGet = () =>
  useQuery({
    queryKey: ['test'],
    queryFn: getTest,
  });
