import { httpClient } from '../../lib/api/httpClient';
import type { ITest } from './ITest';

export const getTest = async (): Promise<ITest> => {
  const { data } = await httpClient.get<ITest>('/tests');
  return data;
};
