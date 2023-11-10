import { createContext } from 'react';
import { AppConfiguration } from './configuration';
import { BalsamAPI } from '../services/BalsamAPIServices';

export interface AppContextState
{
    config: AppConfiguration,
    balsamApi: BalsamAPI,
    getUserName: () => string,
    getUserGroups: () => Array<string>,
    refreshToken: () => void,
}


const AppContext = createContext<AppContextState | undefined>(undefined);

export default AppContext;