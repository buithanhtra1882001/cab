import { IThemeModel } from "./top-nav.type";

export const THEMES_CONSTANT: IThemeModel[] = [
  {
    name: 'Light',
    slug: 'light',
    image: 'https://craftypixels.com/placeholder-image/800x400/ffffff/ffffff',
    description: 'Màu sắc tươi sáng tương phản cao.'
  },
  {
    name: 'Dark',
    slug: 'dark',
    image: 'https://craftypixels.com/placeholder-image/800x400/333333/333333',
    description: 'Phù hợp cho không gian tối, giúp bảo vệ mắt.'
  },
  {
    name: 'CAB+',
    slug: 'cabplus',
    image: 'https://craftypixels.com/placeholder-image/800x400/009cf0/009cf0',
    description: 'Dành riêng cho hội viên CAB+'
  },
  {
    name: 'Fire CAB',
    slug: 'firecab',
    image: 'https://craftypixels.com/placeholder-image/800x400/ff3f42/ff3f42',
    description: 'Sắp ra mắt'
  }
];