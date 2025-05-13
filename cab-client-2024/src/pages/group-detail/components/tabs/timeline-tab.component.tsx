import React from 'react';
import CreatePost from '../../../../components/create-post/create-post.component';
import { IPostModel } from '../../../../models';
import { GoPeople } from 'react-icons/go';
import { FaRegEye } from 'react-icons/fa';
import { MdOutlinePublic } from 'react-icons/md';

interface TimelineTabProps {
  appendNewPost: (newPost: IPostModel) => void;
  getHashTags: () => Promise<void>;
  detail: any;
}

const TimelineTab: React.FC<TimelineTabProps> = ({ appendNewPost, getHashTags, detail }) => {
  return (
    <div className="flex 2xl:gap-12 gap-10 mt-8 max-lg:flex-col">
      <div className="flex-1 xl:space-y-6 space-y-3">
        {' '}
        <CreatePost
          onCreatedSuccess={async (newPost: IPostModel) => {
            appendNewPost(newPost);
            await getHashTags();
          }}
        />
      </div>
      <div className="lg:w-[400px]">
        <div className="lg:space-y-4 lg:pb-8 max-lg:grid sm:grid-cols-2 max-lg:gap-6 uk-sticky uk-active uk-sticky-fixed">
          {' '}
          <div className="box p-5 px-6">
            <div className="flex items-ce justify-between text-black dark:text-white">
              <h3 className="font-bold text-lg"> Giới thiệu </h3>
            </div>

            <ul className="text-gray-700 space-y-4 mt-4 text-sm dark:text-white/80">
              <li>{detail?.groupDescription}</li>
              <li className="flex items-start gap-3">
                <MdOutlinePublic size={24} />
                <div>
                  <span className="font-semibold text-black dark:text-white uppercase">{detail?.privacySettings} </span>
                  <p> Bất kỳ ai cũng có thể thấy ai trong nhóm và những gì họ đăng.</p>
                </div>
              </li>
              <li className="flex items-start gap-3">
                <FaRegEye size={20} />
                <div>
                  <span className="font-semibold text-black dark:text-white uppercase">
                    {detail?.privacySettings === 'public' ? 'Visible' : 'Hidden'}
                  </span>
                  <p>Bất kỳ ai cũng có thể tìm thấy nhóm này</p>
                </div>
              </li>
              <li className="flex items-center gap-3">
                <GoPeople size={20} />
                <div>
                  Thành viên <span className="font-semibold text-black dark:text-white"> {detail?.memberCount}</span>{' '}
                </div>
              </li>
            </ul>
          </div>
        </div>
      </div>
    </div>
  );
};

export default TimelineTab;
