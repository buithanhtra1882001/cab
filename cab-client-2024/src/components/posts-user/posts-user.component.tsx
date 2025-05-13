import React from 'react';
import InfiniteScroll from 'react-infinite-scroll-component';
import _size from 'lodash/size';
import { Post } from '../post/post.component';
import { IPostModel } from '../../models';
import Loading from '../loading/loading.component';

interface PostsUserProps {
  data: IPostModel[];
  hasMore: boolean;
  onLoadMore: () => void;
}

const PostsUser = ({ data, hasMore, onLoadMore }: PostsUserProps) => {
  return (
    <div>
      {_size(data) ? (
        <InfiniteScroll
          dataLength={data.length}
          next={onLoadMore}
          hasMore={hasMore}
          loader={<Loading classname="mx-auto" />}
          endMessage={null}
          scrollableTarget="body"
        >
          {data?.map((post) => {
            return <Post key={post.id} post={post} isPostUser />;
          })}
        </InfiniteScroll>
      ) : null}
    </div>
  );
};

export default PostsUser;
