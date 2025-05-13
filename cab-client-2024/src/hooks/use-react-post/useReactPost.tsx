import { useState } from 'react';
import { postService } from '../../services/post.service';
import toast from 'react-hot-toast';

export const useReactPost = ({ postId }: { postId: string }) => {
  const [liked, setLiked] = useState<boolean>(false);
  const [disliked, setDisLied] = useState<boolean>(false);

  const handleLikePost = async () => {
    try {
      await postService.likePost(postId);
      setLiked(true);
      toast.success('Bạn đã thích bài viết này thành công');
    } catch (error) {
      //
    }
  };

  const handleDisLikePost = async () => {
    try {
      await postService.diLikePost(postId);
      setDisLied(true);
      toast.success('Bạn đã không thích bài viết này thành công');
    } catch (error) {
      //
    }
  };

  return {
    liked,
    disliked,
    handleLikePost,
    handleDisLikePost,
  };
};
