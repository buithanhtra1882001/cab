/* eslint-disable prefer-template */
/* eslint-disable react/self-closing-comp */
/* eslint-disable prettier/prettier */
import dayjs from 'dayjs';
import React, { useMemo, useState } from 'react';
import { useModal } from '..';
import ModalCreatePost from '../../pages/home/components/modal.create.post';
import { Share } from '../Share';
import { Button } from '../button';
import { useUserProfile } from '../../hooks';
import { sendCommentOfPost } from '../../api';
import { MoreHorizontal, Send, Save, Edit, EyeOff, Flag } from 'lucide-react';
import classNames from 'classnames';
import utc from 'dayjs/plugin/utc';
import { Link } from 'react-router-dom';
import { useComment } from '../../hooks/post/useComment';
import _get from 'lodash/get';
import { useReplyComment } from '../../hooks/post/useReplyComment';
import { postService } from '../../services/post.service';
import { IPostModel, IReplyCommentParams } from '../../models';
import toast from 'react-hot-toast';
import ConfirmReactPost from '../cofirm-react-post/cofirm-react-post.component';
import { useReactPost } from '../../hooks/use-react-post/useReactPost';
import Avatar from '../avatar/avatar.component';
import Loading from '../loading/loading.component';
import PreviewPostDialog from '../preview-post-dialog/preview-post-dialog.componet';
import ReactPlayer from 'react-player';
import { HiThumbDown, HiThumbUp } from 'react-icons/hi';
import { AiFillMessage } from 'react-icons/ai';
import { IoShare } from 'react-icons/io5';
import { useSelector } from 'react-redux';
import { RootState } from '../../configuration';
import PostDonate from '../post-donate/post-donate.component';

dayjs.extend(utc);

const classNamePrefix = 'tt-post-component';

type ReactPostState = {
  visible: boolean;
  react: 'like' | 'dislike' | 'idle';
};

export interface IPostProps {
  post?: IPostModel;
  isPostUser?: boolean;
}

export const PostVideo: React.FC<IPostProps> = (props) => {
  const { post, isPostUser } = props;
  const profileUser = useSelector((state: RootState) => state.features.auth.profile);
  const { profile } = useUserProfile();

  const userId = useMemo(() => {
    return profile?.userId;
  }, [profile]);

  const [pageNumberComment, setPageNumberComment] = useState<number>(1);
  // eslint-disable-next-line unused-imports/no-unused-vars
  const [pageNumberReplyComment, setPageNumberReplyComment] = useState<number>(1);

  const [isShowMoreMenu, setShowMoreMenu] = useState<boolean>(false);
  const [reactPostState, setReactPostState] = useState<ReactPostState>({
    visible: false,
    react: 'idle',
  });

  const [visiblePreview, setVisiblePreview] = useState<boolean>(false);
  const [isShowComment, setShowComment] = useState<boolean>(false);
  const [open, setOpen] = useState<boolean>(false);
  const [isClose, setIsClose] = useState<boolean>(true);
  const [idDetail, setIdDetail] = useState<string>('');
  const [indexViewImage, setIndexViewImage] = useState<number>(-1);

  const [isShowReply, setIsShowReply] = useState<boolean>(false);
  const [replyCommentId, setReplyCommentId] = useState<string>('');

  const [replyCommentValue, setReplyCommentValue] = useState<string>('');

  // refactor
  const {
    data: commentList,
    loading: loadingComment,
    hasMore: hasMoreComment,
    getCommentWhenComment,
    refreshCommentClient,
  } = useComment({
    pageNumber: pageNumberComment,
    postId: _get(post, 'id', ''),
    visible: isShowComment,
  });

  const {
    data: replyCommentList,
    hasMore: hasMoreReplyComment,
    getReplyCommentWhenReply,
  } = useReplyComment({
    pageNumber: pageNumberReplyComment,
    commentId: replyCommentId,
    visible: isShowReply,
  });

  const { liked, disliked, handleLikePost, handleDisLikePost } = useReactPost({ postId: _get(post, 'id', '') });
  // refactor

  const { showModal, modalPlaceholder } = useModal();

  const [commentValue, setCommentValue] = useState<string>('');

  const handleEditPost = (e, id) => {
    setOpen(true);
    setShowMoreMenu(false);
    setIsClose(false);
    e.stopPropagation();
    setIdDetail(id);
  };

  const handleClose = (data) => {
    setIsClose(data);
  };

  const handleClearIdDetail = () => {
    setIdDetail('');
  };

  const handleOpen = () => {
    setOpen(false);
  };

  const showModalSharePreview = () => {
    showModal({
      title: 'Viết bài',
      content: <Share post={post} />,
      className: 'lg:min-w-[30%!important] h-[85vh] lg:h-[85vh] overflow-y-auto rounded-md',
    });
  };

  // hidden post
  const [isHiddenPost, setIsHiddenPost] = useState('-1');

  const [isReplyComment, setIsReplyComment] = useState<boolean>(false);
  const [commentId, setCommentId] = useState<string>('');

  const handleCreateReplyComments = (id: string) => {
    if (!commentId) {
      setIsReplyComment(true);
    }

    if (id === commentId) {
      setIsReplyComment((prev) => !prev);
    } else {
      setIsReplyComment(true);
    }

    setCommentId(id);
  };

  const handleSendRelyComment = async (commentId: string) => {
    if (!replyCommentValue) {
      return;
    }

    setReplyCommentId(commentId);

    try {
      const payload: IReplyCommentParams = {
        content: replyCommentValue,
        replyToCommentId: commentId,
      };

      await postService.replyComment(payload);

      setIsShowReply(true);
      setReplyCommentValue('');

      if (isShowReply) {
        await getReplyCommentWhenReply(commentId);
      }

      await refreshCommentClient();
    } catch (error) {
      //
    }
  };

  const handleSendComment = (post) => {
    if (!commentValue) {
      return;
    }
    sendComment({ userId, postId: post?.id, content: commentValue });
  };

  const sendComment = async (data) => {
    const res = await sendCommentOfPost(data);
    if (res) {
      await getCommentWhenComment();
      setCommentValue('');
    }
  };

  const onLikePost = () => {
    if (post?.currentUserHasVoteUp || post?.currentUserHasVoteDown || liked || disliked) {
      toast.success('Bạn đã tương tác với bài viết này!', {
        position: 'top-right',
      });
      return;
    }

    setReactPostState({
      visible: true,
      react: 'like',
    });
  };

  const onDisLikePost = () => {
    if (post?.currentUserHasVoteUp || post?.currentUserHasVoteDown || liked || disliked) {
      toast.success('Bạn đã tương tác với bài viết này!', {
        position: 'top-right',
      });
      return;
    }

    setReactPostState({
      visible: true,
      react: 'dislike',
    });
  };

  const onConfirmReact = async () => {
    const { react } = reactPostState;

    switch (react) {
      case 'idle':
        break;
      case 'dislike':
        await handleDisLikePost();
        break;
      case 'like':
        await handleLikePost();
        break;

      default:
        break;
    }

    setReactPostState({
      visible: false,
      react: 'idle',
    });
  };

  if (isHiddenPost === post?.id) {
    return null;
  }

  return (
    <>
      {reactPostState.visible ? (
        <ConfirmReactPost
          isLike={reactPostState.react === 'like'}
          visible={reactPostState.visible}
          onClose={() =>
            setReactPostState({
              visible: false,
              react: 'idle',
            })
          }
          onCancel={() =>
            setReactPostState({
              visible: false,
              react: 'idle',
            })
          }
          onConfirm={onConfirmReact}
        />
      ) : null}

      {visiblePreview ? (
        <PreviewPostDialog
          visible={visiblePreview}
          onclose={() => setVisiblePreview(false)}
          post={post as IPostModel}
          initialSlide={indexViewImage}
        />
      ) : null}

      <div className="bg-white rounded-xl shadow-sm text-sm font-medium border1 dark:bg-dark2">
        <div className="flex transition duration-200 dark:bg-zinc-900 flex-col">
          <div className="p-4 grid grid-cols-6 gap-4">
            <div className="col-span-4">
              {post?.postType === 'video' && (
                <ReactPlayer width="100%" height="100%" playing url={post?.postVideoResponses[0]?.url} loop controls />
              )}
            </div>
            <div className="w-full lg:w-auto col-span-2 ">
              <div className="flex justify-between items-center">
                <div className="flex">
                  <Link className="mr-4 cursor-pointer" to={`/user/${post?.userId}`}>
                    <Avatar avatar={_get(post, 'userAvatar', '')} label={_get(post, 'userFullName', '')} size="sm" />
                  </Link>

                  <div>
                    <Link
                      className="inline-block text-slate-800 dark:text-slate-50 text-sm font-bold mb-2"
                      to={`/user/${post?.userId}`}
                    >
                      {post?.userFullName ?? 'Người dùng không xác định'}
                    </Link>

                    <div className="flex items-center space-x-2 text-xs">
                      <span className="text-branding font-semibold mr-2">Công nghệ</span>
                      <span>•</span>
                      <span>{dayjs.utc(post?.createdAt).fromNow()} trước</span> <span />
                    </div>
                  </div>
                </div>

                {!isPostUser ? (
                  <div className="relative flex items-start">
                    <Button
                      className="rounded-md flex justify-center items-center gap-2 h-8 text-xs px-2 py-1 transition duration-200 text-zinc-800 dark:text-zinc-50"
                      type="button"
                      onClick={() => setShowMoreMenu((prev) => !prev)}
                    >
                      <MoreHorizontal size={16} />
                    </Button>

                    {isShowMoreMenu && (
                      <div className="absolute top-10 right-0 rounded-md w-max bg-white dark:bg-black text-slate-800 dark:text-slate-50 shadow p-1">
                        <ul>
                          {post?.userId === userId && (
                            <li>
                              <button
                                className="rounded-md flex items-center space-x-2 w-full p-2 hover:bg-gray-1 dark:hover:bg-gray-10 "
                                onClick={(e) => handleEditPost(e, post?.id)}
                              >
                                <Edit size={14} />
                                <span className="text-xs">Chỉnh sửa bài viết</span>
                              </button>
                              <div className="text-xs text-center font-medium " />
                            </li>
                          )}
                          <li>
                            <button className="rounded-md flex items-center space-x-2 w-full p-2 hover:bg-gray-1 dark:hover:bg-gray-10 ">
                              <Save size={14} />
                              <span className="text-xs">Lưu bài</span>
                            </button>
                            <div className="text-xs text-center font-medium " />
                          </li>
                          <li>
                            <button
                              className="rounded-md flex items-center space-x-2 w-full p-2 hover:bg-gray-1 dark:hover:bg-gray-10"
                              onClick={() => {
                                post?.id && setIsHiddenPost(post?.id);
                              }}
                            >
                              <EyeOff size={14} />
                              <span className="text-xs">Ẩn bài viết</span>
                            </button>
                          </li>
                          <li>
                            <button className="rounded-md flex items-center space-x-2 w-full p-2 hover:bg-gray-1 dark:hover:bg-gray-10">
                              <Flag size={14} />
                              <span className="text-xs">Báo cáo vi phạm</span>
                            </button>
                          </li>
                        </ul>
                      </div>
                    )}
                  </div>
                ) : null}
              </div>

              {open && (
                <ModalCreatePost
                  onClearIdDetail={() => handleClearIdDetail()}
                  isOpen={open}
                  onOpen={handleOpen}
                  onClose={handleClose}
                  isClose={isClose}
                  isEdit={idDetail}
                />
              )}

              <div className="mt-4">
                <p className="text-sm text-zinc-800 dark:text-zinc-50">{post?.content}</p>
              </div>

              <ul className="flex text-branding text-sm py-4">
                {post?.hashtags &&
                  post?.hashtags?.split(' ').map((ht, index) => {
                    return (
                      <li className="mr-1 text-violet-800 dark:text-violet-500" key={index}>
                        #{ht}
                      </li>
                    );
                  })}
              </ul>

              <div className="sm:p-4 p-2.5 flex items-center gap-4 text-xs font-semibold">
                <div>
                  <div className="flex items-center gap-2.5">
                    <button
                      type="button"
                      className={classNames(`button-icon   dark:bg-slate-700`, {
                        'text-blue-500 bg-blue-100': post?.currentUserHasVoteUp || liked,
                        'text-slate-800 bg-slate-200 dark:text-slate-50': !post?.currentUserHasVoteUp && !liked,
                      })}
                      onClick={() => {
                        onLikePost();
                      }}
                    >
                      <HiThumbUp size={16} />
                    </button>
                    {_get(post, 'voteUpCount', 0) + (liked ? 1 : 0)}
                  </div>
                </div>
                <div>
                  <div className="flex items-center gap-2.5">
                    <button
                      type="button"
                      className={classNames(`button-icon   dark:bg-slate-700`, {
                        'text-red-500 bg-red-100': post?.currentUserHasVoteUp || liked,
                        'text-slate-800 bg-slate-200 dark:text-slate-50': !post?.currentUserHasVoteUp && !liked,
                      })}
                      onClick={() => {
                        onDisLikePost();
                      }}
                    >
                      <HiThumbDown size={16} />
                    </button>
                    {_get(post, 'voteDownCount', 0) + (liked ? 1 : 0)}
                  </div>
                </div>
                <div>
                  <div className="flex items-center gap-2.5">
                    <button
                      type="button"
                      className="button-icon bg-slate-200/70 dark:bg-slate-700"
                      onClick={() => {
                        setShowComment(!isShowComment);
                        setPageNumberComment(1);
                      }}
                    >
                      <p className="flex items- gap-3 text-slate-800 dark:text-slate-50">
                        <AiFillMessage size={16} />
                      </p>
                    </button>
                    {post?.commentsCount}
                  </div>
                </div>
                <div className="ml-auto">
                  <PostDonate post={post} />
                </div>
                <button type="button" className="button-icon ml-auto" onClick={showModalSharePreview}>
                  <IoShare size={16} />
                </button>
              </div>

              <div className="sm:p-4 p-2.5 border-t border-gray-100 font-normal space-y-3 relative dark:border-slate-700/40">
                {commentList.map((comment, index) => {
                  const { id, userFullName, avatar, content, userId } = comment;
                  return (
                    <div key={index} className="overflow-hidden">
                      <div className={classNames(`flex items-start `)}>
                        <div className="w-full z-10">
                          <div className="flex">
                            <Link to={`/user/${userId}`} className="cursor-pointer">
                              <Avatar avatar={avatar} size="xs" label={userFullName} />
                            </Link>

                            <div className="ml-4 flex-1 mb-2">
                              <div>
                                {userFullName ? (
                                  <Link
                                    to={`/user/${userId}`}
                                    className="text-black font-medium inline-block dark:text-white"
                                  >
                                    {userFullName}
                                  </Link>
                                ) : (
                                  <p className="text-black font-medium inline-block dark:text-white">
                                    Người dùng không xác định
                                  </p>
                                )}
                                <p className="mt-0.5">{content}</p>
                              </div>
                              <div className="flex items-baseline justify-star">
                                <div className="flex justify-end items-baseline text-slate-800 dark:text-slate-50">
                                  <span className="text-xs ">{dayjs.utc(comment.createdAt).local().fromNow()}</span>
                                  <p
                                    className="cursor-pointer text-xs ml-3 font-medium"
                                    onClick={() => handleCreateReplyComments(comment?.id)}
                                  >
                                    Phản hồi
                                  </p>
                                </div>

                                {comment?.totalReply ? (
                                  <div className="text-right ml-4">
                                    <span
                                      className="cursor-pointer text-xs mb-3 text-slate-800 dark:text-slate-50 font-medium"
                                      onClick={() => {
                                        setIsShowReply((prev) => !prev);
                                        setReplyCommentId(comment.id);
                                      }}
                                    >
                                      {isShowReply && comment.id === replyCommentId
                                        ? 'Thu gọn'
                                        : `Xem tất cả ${comment.totalReply} hồi`}
                                    </span>
                                  </div>
                                ) : null}
                              </div>
                            </div>
                          </div>

                          {isShowReply && comment.id === replyCommentId ? (
                            <div className="space-y-2 ml-16">
                              {replyCommentList?.map((reply, index) => (
                                <div key={index} className="flex items-start gap-3 mt-2">
                                  <Link to={`/user/${reply.userId}`} className="cursor-pointer">
                                    <Avatar avatar={reply.avatar} size="xs" label={reply.userFullName} />
                                  </Link>

                                  <div className="flex-1">
                                    <Link
                                      to={`/user/${reply.userId}`}
                                      className="text-black font-medium inline-block dark:text-white"
                                    >
                                      {reply.userFullName}
                                    </Link>

                                    <p className="">{reply.content}</p>

                                    <p className="text-xs  text-right mt-1 text-slate-800 dark:text-slate-50">
                                      {dayjs.utc(reply.createdAt).local().fromNow()}
                                    </p>
                                  </div>
                                </div>
                              ))}
                            </div>
                          ) : null}
                        </div>
                      </div>

                      {isReplyComment && commentId === id ? (
                        <div className="ml-12 flex items-center gap-2 box rounded-md p-4">
                          <input
                            className="outline-none rounded-md w-full text-sm px-4 transition-all duration-200 placeholder:text-sm dark:bg-zinc-800 py-2 placeholder:text-slate-400 text-slate-800 dark:text-zinc-50"
                            placeholder="Nhập bình luận"
                            onChange={(e) => setReplyCommentValue(e.target.value)}
                            value={replyCommentValue}
                          />

                          <button
                            onClick={() => handleSendRelyComment(comment.id)}
                            className="button-icon p-2 hover:bg-blue-100 text-blue-600 cursor-pointer opacity-60 hover:opacity-100"
                          >
                            <Send size={16} />
                          </button>
                        </div>
                      ) : null}
                    </div>
                  );
                })}

                {loadingComment ? <Loading /> : null}

                {hasMoreComment ? (
                  <button
                    className="text-sm text-slate-800 dark:text-slate-50 font-medium cursor-pointer"
                    onClick={() => setPageNumberComment((prev) => prev + 1)}
                  >
                    Xem thêm
                  </button>
                ) : null}
              </div>
              <div className="sm:px-4 sm:py-3 p-2.5 border-t border-gray-100 flex items-center gap-1 dark:border-slate-700/40">
                <img
                  className="w-6 h-6 rounded-full"
                  src={_get(profileUser, 'avatar', '') || 'https://www.w3schools.com/howto/img_avatar.png'}
                  alt="Woman looking front"
                />
                <div className="flex-1 relative overflow-hidden h-10">
                  <input
                    className="outline-none  rounded-md w-full text-sm px-4
          transition-all duration-200 placeholder:text-sm dark:bg-zinc-800 py-2    placeholder:text-slate-400 text-slate-800 dark:text-zinc-50"
                    placeholder="Nhập bình luận"
                    onChange={(e) => {
                      setCommentValue(e.target.value);
                    }}
                    value={commentValue}
                  />
                </div>
                <div className="button-icon p-2  hover:bg-blue-100 text-blue-600 cursor-pointer opacity-60 hover:opacity-100">
                  <Send size={16} />
                </div>
              </div>
            </div>
            <div />
          </div>
        </div>
      </div>
    </>
  );
};
