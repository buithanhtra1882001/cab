/* eslint-disable react/react-in-jsx-scope */
import { useEffect, useMemo, useState } from 'react';
import { ModalProps } from './modal.create.post.type';
import { useDropzone } from 'react-dropzone';
import ReactPlayer from 'react-player';
import {
  createNewPost,
  editOldPost,
  fetchPostCategoriesApi,
  fetchUrlImages,
  fetchUrlVideos,
  getDetailPost,
} from '../../../api';
import { useUserProfile } from '../../../hooks';
import DialogComponent from '../../../components/dialog/dialog.component';
import TextareaAutosize from 'react-textarea-autosize';
import Select, { IOption } from '../../../select/select.component';
import toast from 'react-hot-toast';
import { Image, Trash } from 'lucide-react';
import Autocomplete, { AutocompleteItem } from '../../../components/autocomplete/autocomplete';
import { useHashTag } from '../../../hooks/hashtag/useHashtag';
import ButtonPrimary from '../../../components/button-refactor/button-primary';
import Avatar from '../../../components/avatar/avatar.component';
import _get from 'lodash/get';
import { RootState } from '../../../configuration';
import { useSelector } from 'react-redux';
import { convertPostCategory } from '../../../utils/convertData.util';

interface PostCategory {
  id: string;
  slug: string;
  name: string;
  description: string;
  thumbnail: string;
  status: number;
  isSoftDeleted: boolean;
  createdAt: Date;
  updatedAt: Date;
}

const ModalCreatePost = (props: ModalProps) => {
  const { isOpen, reset, onCreatePost, isEdit, onClose, onClearIdDetail, onOpen, onFetch } = props;

  const [open, setOpen] = useState(isOpen);
  const [textAreaContent, setTextAreaContent] = useState<string>('');
  const [isUploadImage, setIsUploadImage] = useState(false);
  const [isHashTag, setIsHashTag] = useState(false);
  const [isChangeCategory, setIsChangeCategory] = useState(false);
  const [imageIds, setImageIds] = useState<string[]>([]);
  const [imageUrls, setImageUrls] = useState<string[]>([]);
  const [videoIds, setVideoIds] = useState<string[]>([]);
  const [videoUrls, setVideoUrls] = useState<string[]>([]);
  const [postCategory, setPostCategories] = useState<PostCategory[]>([]);
  const [categoryId, setCategoryId] = useState<string>('');
  const [postType, setPostType] = useState<string>('normal');
  const [detailPostData, setDetailPostData] = useState<any>({});
  const [idPostDetail, setIdPostDetail] = useState(isEdit);
  const { profile } = useUserProfile();
  const profileUser = useSelector((state: RootState) => state.features.auth.profile);
  const userId = profile?.userId;

  const handleClose = () => {
    reset?.(false);

    if (onOpen) {
      onOpen();
    }

    if (onClose) {
      onClose(true);
    }
    if (onClearIdDetail) {
      onClearIdDetail();
    }
    setIdPostDetail('');
  };

  const handleChangTextArea = (e) => {
    setTextAreaContent(e.target.value);
  };

  const [loadingUpload, setLoadingUpload] = useState<boolean>(false);
  const [files, setFiles] = useState<File[]>([]);
  const [preview, setPreview] = useState<string[]>([]);
  const [hashTags, setHashTags] = useState<string>('');

  const onDrop = (acceptedFiles: File[]) => {
    const previewUrls = acceptedFiles.map((file) => {
      return URL.createObjectURL(file);
    });

    setFiles((prev) => [...prev, ...acceptedFiles]);
    setPreview([...preview, ...previewUrls]);
  };

  const { getRootProps, getInputProps } = useDropzone({
    onDrop,
  });

  const handleClearImage = (obj, link) => {
    const newListFile = files.filter((file) => file !== obj);
    setFiles(newListFile);
    const newPreview = preview.filter((url) => url !== link);
    setPreview(newPreview);
  };

  useEffect(() => {
    const handleUploadImages = async () => {
      try {
        setLoadingUpload(true);
        const handles = files.map((item) => {
          if (item?.type.startsWith('image')) {
            return handleGetUrlFileImage(item);
          }
          if (item?.type.startsWith('video')) {
            setPostType('video');
            return handleGetUrlFileVideo(item);
          }
          return null;
        });

        await Promise.all(handles);
      } catch (error) {
        //
      } finally {
        setLoadingUpload(false);
      }
    };

    handleUploadImages();
  }, [files.length]);

  const handleGetUrlFileImage = async (item) => {
    const formData = new FormData();
    formData.append('type', item);
    const res = await fetchUrlImages(formData);
    if (res.length) {
      res.forEach((img) => {
        setImageIds((prev) => [...prev, img?.id]);
        setImageUrls((prev) => [...prev, img?.url]);
      });
    }
  };

  const handleGetUrlFileVideo = async (item) => {
    const formData = new FormData();
    formData.append('type', item);
    const res = await fetchUrlVideos(formData);
    if (res.length > 0) {
      res.forEach((video) => {
        setVideoIds([...videoIds, video?.id]);
        setVideoUrls([...videoUrls, video?.url]);
      });
    }
  };

  useEffect(() => {
    handleGetPostCategories();
  }, []);

  const handleGetPostCategories = async () => {
    const res = await getCategory();
    setPostCategories(res as unknown as PostCategory[]);
  };

  const getCategory = async () => {
    const res = await fetchPostCategoriesApi();
    return res;
  };

  const validateForm = () => {
    if (!textAreaContent) {
      toast.error('Nhập nội dung', {
        position: 'top-right',
      });
      return false;
    }

    if (!categoryId) {
      toast.error('Chọn danh mục', {
        position: 'top-right',
      });
      return false;
    }
    if (!hashTags) {
      toast.error('Vui lòng nhập hashtag', {
        position: 'top-right',
      });
      return false;
    }
    return true;
  };

  const handleCreateNewPost = async () => {
    if (!validateForm()) {
      return;
    }

    const imageInfo = imageIds.map((id, index) => {
      return {
        imageId: id,
        imageUrl: imageUrls[index],
      };
    });

    const videoInfo = videoIds.map((id, index) => {
      return {
        videoId: id,
        videoUrl: videoUrls[index],
      };
    });

    const data = {
      userId,
      postType,
      categoryId,
      hashtags: hashTags,
      content: textAreaContent,
      title: textAreaContent,
      imageInfo,
      videoInfo,
    };

    const newPost = await createNewPost(data);

    setOpen(false);
    onCreatePost?.(newPost);
  };

  useEffect(() => {
    if (idPostDetail) {
      handleGetDetailPost(idPostDetail);
    }
  }, [idPostDetail]);

  const handleGetDetailPost = async (postId) => {
    const res = await getDetailPost(postId);
    setDetailPostData(res);
  };

  useEffect(() => {
    setTextAreaContent(detailPostData.content);
    setCategoryId(detailPostData.categoryId);
    if (typeof detailPostData?.hashtags === 'string') {
      setHashTags(detailPostData?.hashtags);
    } else if (detailPostData?.hashtags?.isArray()) {
      const listHashtagArray = detailPostData?.hashtags;
      const hashtags = listHashtagArray?.join(', ');
      setHashTags(hashtags);
    }
  }, [detailPostData]);

  const handleEditPost = () => {
    const data = {
      postId: idPostDetail,
      content: textAreaContent,
      imageUrls,
      videoUrls,
    };
    editPost(data);
  };

  const editPost = async (data) => {
    const res = await editOldPost(data);
    if (res) {
      if (onOpen) {
        onOpen();
      }
      if (onFetch) {
        onFetch();
      }
    }
  };

  const categoryOptions = useMemo(() => {
    const options = postCategory.map((category) => {
      const res: IOption = {
        label: category.name,
        value: category.id,
      };
      return res;
    });

    return options;
  }, [postCategory]);

  const { hashtags: hashtagsResponse, searchHashTags } = useHashTag();

  const hashTagOptions = useMemo(() => {
    if (!hashtagsResponse) {
      return [];
    }

    const res: AutocompleteItem[] = hashtagsResponse.map((hashtag) => {
      const option: AutocompleteItem = {
        label: hashtag.name,
        value: hashtag.name,
      };

      return option;
    });

    return res;
  }, [hashtagsResponse]);

  return (
    <DialogComponent
      onClose={() => handleClose()}
      visible={open}
      placement="CENTER"
      title={!idPostDetail ? 'Tạo bài viết' : 'Chỉnh sửa bài viết'}
      size="xl"
    >
      <div className="w-full pb-5">
        <div className="">
          <div className="flex py-2">
            <Avatar avatar={_get(profile, 'avatar', '')} label={_get(profile, 'fullname', '')} size="sm" />
            <div className="ml-2">
              <h3 className="text-slate-800 dark:text-slate-50 font-semibold text-sm">
                {_get(profileUser, 'fullname', '')}
              </h3>
              {!isChangeCategory && (
                <p
                  onClick={() => setIsChangeCategory(true)}
                  className="text-slate-600 dark:text-slate-50 text-xs cursor-pointer border border-solid rounded-[4px] px-2 py-1 "
                >
                  {categoryOptions.find((item: any) => item.value === categoryId)?.label
                    ? categoryOptions.find((item: any) => item.value === categoryId)?.label
                    : 'Chọn danh mục'}
                </p>
              )}
              {isChangeCategory &&
                (!idPostDetail ? (
                  <Select
                    options={categoryOptions}
                    onChange={(data: any) => {
                      setCategoryId(data.value);
                      setIsChangeCategory(false);
                    }}
                    placeholder="Chọn danh mục"
                  />
                ) : (
                  <span className="outline-none border border-[#1D8DE3] text-[#1D8DE3] rounded-[4px] float-right min-w-[100px] text-center">
                    {convertPostCategory(categoryId)}
                  </span>
                ))}
            </div>
          </div>

          <div className="w-full border border-solid rounded-lg p-4 pb-12">
            <div className="flex gap-2 pb-5">
              <TextareaAutosize
                className="outline-none  text-slate-800 dark:text-slate-50 bg-zinc-50 dark:bg-transparent
              rounded-md w-full resize-none text-sm"
                onChange={(e) => handleChangTextArea(e)}
                value={textAreaContent}
                placeholder="Bạn đang nghĩ gì?"
                autoFocus
              >
                {textAreaContent}
              </TextareaAutosize>
            </div>
          </div>
          <div className="py-2">
            {!idPostDetail && hashtagsResponse ? (
              <Autocomplete
                items={hashTagOptions}
                onChange={(value) => {
                  setHashTags(value as string);
                }}
                onNoneItem={(inputValue) => {
                  searchHashTags(inputValue);
                }}
                placeholder="Hashtag"
              />
            ) : (
              <span className="inline-block flex-1 text-xs">{hashTags}</span>
            )}
          </div>
          <div className="flex space-x-4 py-2">
            <div>
              <button
                onClick={() => setIsUploadImage(!isUploadImage)}
                className="text-gray-900 bg-white hover:bg-gray-100 border border-gray-200  focus:outline-none font-medium rounded-lg text-sm p-2 text-center inline-flex items-center    "
              >
                <Image />
              </button>
            </div>
          </div>

          {isUploadImage && (
            <div>
              <div
                className="w-full min-h-28 py-5 flex justify-center items-center cursor-pointer rounded-md z-10
              border border-dotted border-zinc-400 dark:border-zinc-50"
              >
                <div className={`absolute top-0 right-0 z-50 ${preview.length === 0 ? 'hidden' : ''}`}>
                  <label
                    htmlFor="upload-post-second"
                    className="z-10 bg-[rgba(63,62,62,0.5)] text-slate-800 dark:text-slate-50 cursor-pointer"
                  >
                    Thêm ảnh/video
                  </label>
                  <div {...getRootProps()} className="hidden">
                    <input {...getInputProps()} id="upload-post-second" type="file" accept="image/*" multiple />
                  </div>
                </div>

                <div className="">
                  {preview.length > 0 ? (
                    <div>
                      <div className="w-full h-full grid grid-cols-2 px-4 gap-3">
                        {files?.map((item, index) => {
                          if (item?.type?.startsWith('image')) {
                            return (
                              <div key={index} className="relative">
                                <img
                                  src={preview[index]}
                                  alt="Preview"
                                  className="inline-block object-cover w-full h-48"
                                />
                                <span
                                  onClick={() => handleClearImage(item, preview[index])}
                                  className="cursor-pointer absolute rounded-md right-2 top-2 text-slate-800 w-6 h-6
                                flex justify-center items-center bg-slate-50"
                                >
                                  <Trash size={14} />
                                </span>
                              </div>
                            );
                          }
                          return (
                            <div key={index} className="relative w-[100px] h-[100px] mr-[20px] overflow-hidden">
                              <ReactPlayer url={preview} controls className="object-fill w-full h-full" />
                              <span
                                onClick={() => handleClearImage(item, preview[index])}
                                className="cursor-pointer absolute bg-[#E4E6EB] w-[20px] h-[20px] rounded-full left-[0px] top-[0px] text-[30px] flex justify-center items-center"
                              >
                                <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 50 50" width="10px" height="10px">
                                  <path d="M 7.71875 6.28125 L 6.28125 7.71875 L 23.5625 25 L 6.28125 42.28125 L 7.71875 43.71875 L 25 26.4375 L 42.28125 43.71875 L 43.71875 42.28125 L 26.4375 25 L 43.71875 7.71875 L 42.28125 6.28125 L 25 23.5625 Z" />
                                </svg>
                              </span>
                            </div>
                          );
                        })}
                      </div>
                    </div>
                  ) : (
                    <div {...getRootProps()} className="flex items-center justify-center">
                      <input {...getInputProps()} id="upload-post" type="file" accept="image/*" multiple />
                      <label htmlFor="upload-post" className="cursor-pointer flex justify-center items-center flex-col">
                        <p className="font-medium text-sm text-slate-800 dark:text-slate-50">Ảnh/video</p>
                        <p className="mt-2 text-sm text-slate-600 dark:text-slate-50">(hoặc kéo và thả)</p>
                      </label>
                    </div>
                  )}
                </div>
              </div>
            </div>
          )}

          {!idPostDetail ? (
            <ButtonPrimary
              onClick={handleCreateNewPost}
              className="w-full mt-4 flex justify-center"
              disabled={loadingUpload}
            >
              Đăng bài
            </ButtonPrimary>
          ) : (
            <ButtonPrimary
              onClick={handleEditPost}
              className="w-full mt-4 flex justify-center"
              disabled={loadingUpload}
            >
              Lưu
            </ButtonPrimary>
          )}
        </div>
      </div>
    </DialogComponent>
  );
};

export default ModalCreatePost;
