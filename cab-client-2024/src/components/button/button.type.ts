import { ITotoLink } from '../../models';

export interface IButtonProps extends ITotoLink, React.DOMAttributes<HTMLButtonElement> {
  type?: 'button' | 'link' | 'text';
  children?: React.ReactNode;
}
