export interface ISliderProps {
  sliders?: string[];
  type?: 'sliders' | 'only-image';
  autoNext?: boolean;
}

/**
 * TODO Handle Slide with item
 */

export interface ISlideItem {
  id: number | string;
  imageUrl?: string;
  ctaUrl?: string;
}
