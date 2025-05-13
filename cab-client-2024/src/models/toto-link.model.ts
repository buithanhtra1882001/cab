import { HTMLAttributeAnchorTarget } from 'react';

export interface ITotoLink {
  title?: string;
  link?: string;
  authenRequired?: boolean;
  className?: string;
  target?: HTMLAttributeAnchorTarget;
  disabled?: boolean;
  routeLink?: string;
}
