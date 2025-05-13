import { ITotoLink } from './toto-link.model';

export interface IHeroSlider {
  slides: IHeroSlide[];
  height?: number | string;
  responsiveByRatio?: boolean;
  responsiveRatioWidth?: number;
  responsiveRatioHeight?: number;
}

export interface IHeroSlide {
  id: number | string;
  imageUrl?: string;
  ctaUrl?: string; //! deprecated, use link instead
  link?: ITotoLink;
  mobileImageUrl?: string;
  backgroundPosition?: string;
}
