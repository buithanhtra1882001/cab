import 'slick-carousel/slick/slick.css';
import 'slick-carousel/slick/slick-theme.css';

import React, { useMemo, useRef } from 'react';
import { IPostModel } from '../../models';
import DialogComponent from '../dialog/dialog.component';
import Slider, { Settings } from 'react-slick';
import { ChevronLeft, ChevronRight } from 'lucide-react';
import { LazyLoadImage } from '../lazy-load-image';

interface PreviewPostDialogProps {
  post: IPostModel;
  visible: boolean;
  initialSlide: number;
  onclose: () => void;
}

const PreviewPostDialog = ({ visible, post, initialSlide, onclose }: PreviewPostDialogProps) => {
  const sliderRef = useRef<Slider>(null);

  const settings: Settings = useMemo(() => {
    const result: Settings = {
      dots: false,
      infinite: true,
      speed: 500,
      slidesToShow: 1,
      slidesToScroll: 1,
      initialSlide,
      arrows: false,
    };

    return result;
  }, [initialSlide]);

  const images = useMemo(() => {
    const res = post.postImageResponses.map((image) => image);

    return res;
  }, [post]);

  if (!visible) {
    return null;
  }

  return (
    <div>
      <DialogComponent visible={visible} onClose={onclose} title={post.userFullName} size="lg">
        <div className="mt-5">
          <p className="text-slate-800 dark:text-slate-50 text-sm line-clamp-2">{post.title}</p>

          <div className="py-10 relative">
            <Slider {...settings} ref={sliderRef}>
              {images.map((image) => (
                <div key={image.imageId} className="max-h-[720px] h-[500px] !flex items-center justify-center">
                  <LazyLoadImage src={image.url} alt={image.url} />
                </div>
              ))}
            </Slider>

            <div>
              <ChevronLeft
                size={28}
                onClick={() => sliderRef.current?.slickPrev()}
                className="cursor-pointer absolute -left-7 top-1/2 transform -translate-y-1/2 text-slate-800 dark:text-slate-50"
              />
              <ChevronRight
                size={28}
                onClick={() => sliderRef.current?.slickNext()}
                className="cursor-pointer absolute -right-7 top-1/2 transform -translate-y-1/2 text-slate-800 dark:text-slate-50"
              />
            </div>
          </div>
        </div>
      </DialogComponent>
    </div>
  );
};

export default PreviewPostDialog;
