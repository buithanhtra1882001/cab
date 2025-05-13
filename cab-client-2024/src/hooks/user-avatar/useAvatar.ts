import _get from 'lodash/get';
import _toString from 'lodash/toString';
import { useCallback, useState } from 'react';

export const useAvatar = () => {
  const [visibleUpload, setVisibleUpload] = useState<boolean>(false);
  const [blob, setBlob] = useState<Blob | null>(null);
  const [fileName, setFileName] = useState<string>('');
  const [imageReviewSrc, setImageReviewSrc] = useState<string>('');
  const [refreshInputFile, setRefreshInputFile] = useState<string>('');

  const getBlob = (blob: Blob) => {
    setBlob(blob);
  };

  const onInputChange = useCallback(({ currentTarget }: React.ChangeEvent<HTMLInputElement>) => {
    const file = _get(currentTarget, 'files[0]') as unknown as File;

    const reader = new FileReader();

    reader.addEventListener(
      'load',
      () => {
        setFileName(file.name);
        setImageReviewSrc(_toString(reader.result));

        const img = new Image();
        img.src = _toString(reader.result);
        img.onload = () => {
          setVisibleUpload(true);
          setRefreshInputFile(_toString(new Date().getTime()));
        };
      },
      false,
    );

    if (file) {
      reader.readAsDataURL(file);
    }
  }, []);

  const onClose = () => {
    setImageReviewSrc('');
    setVisibleUpload(false);
  };

  return {
    visibleUpload,
    fileName,
    blob,
    refreshInputFile,
    imageReviewSrc,
    getBlob,
    onInputChange,
    onClose,
  };
};
