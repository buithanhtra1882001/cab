/* eslint-disable react/self-closing-comp */
import React, { memo, useEffect, useRef, useState } from 'react';
import * as yup from 'yup';
import { SubmitHandler, useForm } from 'react-hook-form';
import { yupResolver } from '@hookform/resolvers/yup';
import { Send, SmilePlus } from 'lucide-react';
import ReactTextareaAutosize from 'react-textarea-autosize';
import { IFriend } from '../../types/friend';
import { SendMessagePayload } from '../../types/chat';
import EmojiPicker, { EmojiClickData, Theme } from 'emoji-picker-react';
import { useOnClickOutside } from 'usehooks-ts';
import { useDarkMode } from '../../hooks';

interface IChatEditorSchema {
  chatContent: string;
}

interface ChatEditorProps {
  user: IFriend;
  handleSendMessage: (payload: SendMessagePayload) => void;
}

const editorId = 'input-chat-editor';
const editorSubmitButton = 'button-submit-editor';

const chatEditorSchema = yup.object().shape({
  chatContent: yup.string().required(),
});

const ChatEditor = ({ user, handleSendMessage }: ChatEditorProps) => {
  const emojiRef = useRef<HTMLDivElement>(null);

  const { isDarkMode } = useDarkMode();

  const [visibleEmoji, setVisibleEmoji] = useState<boolean>(false);

  const { register, handleSubmit, setValue, setFocus, getValues } = useForm<IChatEditorSchema>({
    mode: 'all',
    defaultValues: {
      chatContent: '',
    },
    resolver: yupResolver(chatEditorSchema),
  });

  const onSubmit: SubmitHandler<IChatEditorSchema> = (formData) => {
    try {
      const { chatContent } = formData;

      const payload: SendMessagePayload = {
        RecipientUserId: user.userId,
        Content: chatContent,
      };

      handleSendMessage(payload);
      setValue('chatContent', '');
      setFocus('chatContent');
    } catch (error) {
      console.log(error);
    }
  };

  const handleInsertEmoji = (emoji: EmojiClickData) => {
    const editorInput = document.getElementById(editorId) as HTMLTextAreaElement;
    const cursor = editorInput?.selectionStart;

    const _value = getValues('chatContent');
    const insertedEmoji = _value.slice(0, cursor) + emoji.emoji + _value.slice(cursor);

    setValue('chatContent', insertedEmoji);
    setFocus('chatContent');
  };

  useEffect(() => {
    setFocus('chatContent');
  }, [user]);

  useOnClickOutside(emojiRef, () => {
    setVisibleEmoji(false);
  });

  return (
    <form onSubmit={handleSubmit(onSubmit)} className="flex items-center md:gap-4 gap-2 md:p-3 p-2 overflow-hidden">
      <div className="flex items-center gap-2 h-full dark:text-white -mt-1.5">
        <SmilePlus onClick={() => setVisibleEmoji((prev) => !prev)} className="cursor-pointer mx-4" />
      </div>
      <div className="relative flex-1">
        <ReactTextareaAutosize
          className="w-full resize-none bg-secondery rounded-full px-4 p-5"
          placeholder="Aaa..."
          {...register('chatContent')}
          id={editorId}
          onKeyDown={(e) => {
            if (e.code === 'Enter' && !e.shiftKey) {
              e.preventDefault();
              const buttonSubmit = document.getElementById(editorSubmitButton);

              buttonSubmit?.click();
            }
          }}
        />
      </div>
      <div className="absolute right-4 bottom-16" ref={emojiRef}>
        <EmojiPicker
          open={visibleEmoji}
          onEmojiClick={(emoji) => {
            handleInsertEmoji(emoji);
          }}
          theme={isDarkMode ? Theme.DARK : Theme.LIGHT}
        />
      </div>

      <button className="w-10 h-10 p-0" type="submit" id={editorSubmitButton}>
        <Send />
      </button>
    </form>
  );
};

export default memo(ChatEditor);
