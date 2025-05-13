import React, { useEffect, useState } from 'react';
import { ISliderProps } from './slider.type';
import { LazyLoadImage } from '../lazy-load-image';
import { ArrowLeft, ArrowRight } from 'lucide-react';

export const Slider: React.FC<ISliderProps> = (props) => {
  const { sliders = [], type = 'sliders', autoNext = false } = props;

  const [imgIndexCurrent, setImageIndexCurrent] = useState<number>(0);

  const handleNextSlider = (data: number) => {
    if (data === -1) {
      setImageIndexCurrent((pv) => {
        if (pv === 0) return sliders.length - 1;
        return pv - 1;
      });
    } else if (data === 1) {
      setImageIndexCurrent((pv) => {
        if (pv === sliders.length - 1) return 0;
        return pv + 1;
      });
    }
  };

  useEffect(() => {
    const interval = setInterval(() => {
      if (autoNext === false) return;
      if (imgIndexCurrent === sliders.length - 1) {
        setImageIndexCurrent(0);
      } else {
        setImageIndexCurrent((pv) => pv + 1);
      }
    }, 2000);

    return () => clearInterval(interval);
  }, [autoNext, imgIndexCurrent, sliders.length]);

  return (
    <div className="slider-container">
      {type === 'sliders' && (
        <div className="p-4">
          <div className="flex flex-row justify-between h-[500px] w-full">
            <button title="leftClick" onClick={() => handleNextSlider(-1)}>
              <ArrowLeft size={24} />
            </button>
            <div className="h-[500px] w-[700px]">
              <LazyLoadImage
                wrapperClassName="object-contain h-full w-full"
                className="object-contain h-full w-full rounded-lg"
                src={sliders[imgIndexCurrent]}
                alt=""
                effect="blur"
              />
            </div>
            <button
              title="rightClick"
              onClick={() => handleNextSlider(1)}
              // className={imgIndexCurrent === sliders.length - 1 ? 'invisible' : 'visible'}
            >
              <ArrowRight size={24} />
            </button>
          </div>
        </div>
      )}

      {type === 'only-image' && (
        <div
          className="splide splide--slide splide--ltr splide--draggable is-active is-overflow is-initialized"
          aria-label="My Favorite Images"
          id="splide02"
          role="region"
          aria-roledescription="carousel"
        >
          <div
            className="splide__track splide__track--slide splide__track--ltr splide__track--draggable"
            id="splide02-track"
            aria-live="polite"
            aria-atomic="true"
            style={{ paddingLeft: '0px', paddingRight: '0px' }}
          >
            <ul
              className="splide__list"
              id="splide02-list"
              role="presentation"
              style={{ transform: 'translateX(0px)' }}
            >
              <li
                className="splide__slide is-active is-visible"
                id="splide02-slide01"
                role="group"
                aria-roledescription="slide"
                aria-label="1 of 3"
              >
                <LazyLoadImage
                  wrapperClassName="h-56 w-full object-fit"
                  className="object-fit h-56 w-full rounded-lg"
                  src={sliders[imgIndexCurrent]}
                  alt=""
                  effect="blur"
                />
              </li>
            </ul>
          </div>
        </div>
      )}
    </div>
  );
};
