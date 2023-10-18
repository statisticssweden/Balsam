export interface AppContextState
{
    config: AppConfiguration,
    balsamApi: BalsamAPI
}

import { createContext } from 'react';
import { AppConfiguration } from './configuration';
import { BalsamAPI } from '../services/BalsamAPIServices';
const AppContext = createContext<AppContextState | undefined>(undefined);

export default AppContext;