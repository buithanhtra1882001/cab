/* eslint-disable react/self-closing-comp */
import React, { useEffect, useState } from 'react';
import { Helmet } from 'react-helmet-async';
import InfiniteScroll from 'react-infinite-scroll-component';
import { Post } from '../../components';
import _size from 'lodash/size';
import Loading from '../../components/loading/loading.component';
import { usePostVideosHook } from '../../hooks/post/usePostVideos';
import ReactPlayer from 'react-player';
import { usePostVideosSuggestionHook } from '../../hooks/post/usePostVideosSuggestions';
import { PostVideo } from '../../components/post/post-video-component';
import dayjs from 'dayjs';
import { useParams } from 'react-router-dom';
import { usePostDetail } from '../../hooks/post/usePostDetail';
import _ from 'lodash';

export const VideosPage = () => {
  const { id } = useParams();
  const [pageNumber, setPageNumber] = useState<number>(1);
  const { postDetail, loading: postDetailLoading } = usePostDetail({ id });
  // Assuming listPostSuggestion and setVideo are defined elsewhere in your component
  const [videoDurations, setVideoDurations] = useState({});

  // Function to update the duration for a video
  const handleDuration = (duration, postId) => {
    setVideoDurations((prevDurations) => ({
      ...prevDurations,
      [postId]: duration,
    }));
  };
  const [video, setVideo] = useState<any>({});
  const { data: listPost, hasMore, loading } = usePostVideosHook({ pageNumber });
  const { data: listPostSuggestion } = usePostVideosSuggestionHook({
    postId: video?.id,
    pageNumber,
  });

  useEffect(() => {
    window.scrollTo(0, 0);
    if (id && !postDetailLoading && !_.isEmpty(postDetail)) setVideo(postDetail);
    if (!id || !postDetail) setVideo(listPost[0]);
  }, [listPost, id, postDetail]);

  return (
    <div className="lg:flex 2xl:gap-16 gap-12 max-w-[1065px] mx-auto">
      <div className=" dark:bg-black dark:text-zinc-200 pt-5 lg:space-y-8">
        <Helmet>
          <title>Cab VN</title>
          <meta name="home" content="Cab VN" />
        </Helmet>
        <div className="container gap-2  border-b-2">
          <PostVideo post={video} />
        </div>

        <div className="w-full px-2 lg:px-0 mx-auto">
          <div className="flex justify-between lg:space-x-8">
            <main className="w-full">
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
                      return <Post key={post.id} post={post} />;
                    })}
                  </div>
                </InfiniteScroll>
              ) : null}
            </main>

            <div className="lg:max-w-xs 2xl:max-w-sm w-full">
              <aside
                className="lg:space-y-6 space-y-4 lg:pb-8 max-lg:grid sm:grid-cols-2 max-lg:gap-6 sticky"
                style={{
                  height: 'calc(100vh - 84px)',
                }}
              >
                <div className="box p-5 pr-2">
                  <div className="flex items-baseline justify-between text-black dark:text-white">
                    <h3 className="font-bold text-base"> Video gợi ý </h3>
                  </div>
                  <div className="mt-3 space-y-4">
                    {listPostSuggestion?.map((post, index) => {
                      return (
                        <div
                          key={post.id}
                          onClick={() => {
                            setVideo(post);
                          }}
                          className="relative flex lg:flex-row flex-col gap-2.5"
                        >
                          <div className="relative lg:w-[130px] lg:h-[80px] aspect-[3/1.5] overflow-hidden rounded-lg shrink-0">
                            <ReactPlayer
                              onDuration={(duration) => handleDuration(duration, post.id)}
                              url={post?.postVideoResponses[0]?.url}
                              width="100%"
                              height="100%"
                            />
                            <img
                              src="https://demo.foxthemes.net/socialite-v3.0/assets/images/icon-play.svg"
                              className="w-6 h-6 absolute !top-1/2 !left-1/2 -translate-x-1/2 -translate-y-1/2"
                              alt=""
                            />
                            <div className="absolute bottom-1 right-1 bg-black/70 font-normal rounded px-0.5 py-0.5 text-xs text-white">
                              {videoDurations[post.id]
                                ? `${Math.floor(videoDurations[post.id] / 60)}:${Math.floor(
                                    videoDurations[post.id] % 60,
                                  )
                                    .toString()
                                    .padStart(2, '0')}`
                                : '00:00'}
                            </div>
                          </div>
                          <div className="flex-1">
                            <a href="#">
                              {' '}
                              <h3 className="text-sm font-semibold line-clamp-2 mb-1.5"> {post?.title} </h3>
                            </a>
                            <div className="text-xs">
                              <a href="#" className="flex items-center gap-1 mb-0.5">
                                {post?.userFullName}{' '}
                              </a>
                              <div className="flex items-center gap-2">
                                <div> {post?.viewCount} views </div>
                                <div> {dayjs.utc(post?.createdAt).fromNow()} trước</div>
                              </div>
                            </div>
                          </div>
                        </div>
                      );
                    })}
                  </div>
                </div>
              </aside>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};
