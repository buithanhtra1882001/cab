/* eslint-disable react/self-closing-comp */
import React, { useState } from 'react';
import { Helmet } from 'react-helmet-async';
import InfiniteScroll from 'react-infinite-scroll-component';
import { Post } from '../../components';
import { AsideRight } from '../../views/aside-right';
import { usePostHook } from '../../hooks/post/usePost';
import _size from 'lodash/size';
import CreatePost from '../../components/create-post/create-post.component';
import { IPostModel } from '../../models';
import { useHashTag } from '../../hooks/hashtag/useHashtag';
import Loading from '../../components/loading/loading.component';
import { useSelector } from 'react-redux';
import { RootState } from '../../configuration';

export const HomePage = () => {
  const [pageNumber, setPageNumber] = useState<number>(1);
  const profileUser = useSelector((state: RootState) => state.features.auth.profile);
  const { data: listPost, hasMore, loading, appendNewPost } = usePostHook({ pageNumber });
  const { hashtags, getHashTags } = useHashTag();

  return (
    <div className="lg:flex 2xl:gap-16 gap-12 max-w-[1065px] mx-auto">
      <Helmet>
        <title>Cab VN</title>
        <meta name="home" content="Cab VN" />
      </Helmet>
      <div className="w-[680px] mx-auto">
        <main className="w-full">
          <div className="mb-8">
            <CreatePost
              onCreatedSuccess={async (newPost: IPostModel) => {
                appendNewPost(newPost);
                await getHashTags();
              }}
            />
          </div>

          {loading ? <Loading classname="mx-auto" /> : null}

          {_size(listPost) ? (
            <InfiniteScroll
              dataLength={listPost.length}
              next={() => {
                setPageNumber((prev) => prev + 1);
              }}
              hasMore={hasMore}
              loader={<Loading classname="mx-auto" />}
              endMessage={null}
              scrollableTarget="body"
            >
              <div className="flex-1 xl:space-y-6 space-y-3">
                {listPost?.map((post, index) => {
                  return <Post key={post.id} post={post} isPostUser={post.userId === profileUser?.userId} />;
                })}
              </div>
            </InfiniteScroll>
          ) : null}
        </main>
      </div>

      <div className="flex-1">
        <AsideRight hashtags={hashtags || []} />
      </div>
    </div>
  );
};
