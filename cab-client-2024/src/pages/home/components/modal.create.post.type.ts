import { IPostModel } from '../../../models';

export type ModalProps = {
  isOpen: boolean | undefined;
  reset?: Function;
  onCreatePost?: (newPost: IPostModel) => void;
  isClose?: boolean;
  isEdit?: string;
  onClose?: Function;
  onClearIdDetail?: Function;
  onOpen?: Function;
  onFetch?: Function;
};
