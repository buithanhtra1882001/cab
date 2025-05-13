import { ISystemConfigModel } from '../../models';
import { useUIContent } from '../use-ui-content';
import { ISystemConfigHook } from './use-system-config.type';

export const useSystemConfig: ISystemConfigHook = () => {
  const { content, loading } = useUIContent<ISystemConfigModel>('system-config', { language: 'vi' });

  return {
    systemConfig: content,
    loading,
  };
};
