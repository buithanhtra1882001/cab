import { ISystemConfigModel } from '../../models';

export interface ISystemConfigHook {
  (): { systemConfig?: ISystemConfigModel; loading: boolean };
}
