export interface IAsideLeftProps {}

export interface IAsideLeftContentModel {
  asideLeftMenu: IAsideLeftMenuModel[];
}

export interface IAsideLeftMenuModel {
  id: string | number;
  name: string;
  icon?: React.ReactNode;
  link: string;
}
