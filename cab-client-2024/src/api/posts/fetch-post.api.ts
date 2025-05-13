import { API } from '..';
import { FetchPostParams, IPostModel, IPostResponse } from '../../models';

export const fetchPostsApi = (params: FetchPostParams) => {
  return new Promise<IPostResponse>((resolve, reject) => {
    API.post('/v1/post-service/posts/home-post', {
      params,
    })
      .then((res) => {
        resolve(res.data);
      })
      .catch(reject);
  });
};

export const fetchPostCategoriesApi = () => {
  return new Promise<IPostModel[]>((resolve, reject) => {
    API.get('v1/post-service/postcategories')
      .then((res) => {
        resolve(res.data);
      })
      .catch(reject);
  });
};

export const getAllPost = (data) => {
  return new Promise<IPostModel[]>((resolve, reject) => {
    API.post('v1/post-service/posts/home-post', data)
      .then((res) => {
        resolve(res.data);
      })
      .catch(reject);
  });
};

export const fetchUrlImages = (formData) => {
  return new Promise<IPostModel[]>((resolve, reject) => {
    API.post('v1/media-service/images/upload', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    })
      .then((res) => {
        resolve(res.data);
      })
      .catch(reject);
  });
};

export const fetchUrlVideos = (formData) => {
  return new Promise<IPostModel[]>((resolve, reject) => {
    API.post('v1/media-service/video/upload', formData, {
      headers: {
        'Content-Type': 'multipart/form-data',
      },
    })
      .then((res) => {
        resolve(res.data);
      })
      .catch(reject);
  });
};

export const createNewPost = (data) => {
  return new Promise<IPostModel>((resolve, reject) => {
    API.post('v1/post-service/posts', data)
      .then((res) => {
        resolve(res.data);
      })
      .catch(reject);
  });
};

export const getDetailPost = (param: string) => {
  return new Promise<IPostModel[]>((resolve, reject) => {
    API.get(`v1/post-service/posts/${param}`)
      .then((res) => {
        resolve(res.data);
      })
      .catch(reject);
  });
};

export const editOldPost = (data: string) => {
  return new Promise<IPostModel[]>((resolve, reject) => {
    API.put('v1/post-service/posts', data)
      .then((res) => {
        resolve(res.data);
      })
      .catch(reject);
  });
};

export const sendCommentOfPost = (data) => {
  return new Promise<IPostModel[]>((resolve, reject) => {
    API.post('v1/post-service/comments', data)
      .then((res) => {
        resolve(res.data);
      })
      .catch(reject);
  });
};

export const getAllComment = (data) => {
  return new Promise<IPostModel[]>((resolve, reject) => {
    API.post('v1/post-service/comments/get-comment-by-postId', data)
      .then((res) => {
        resolve(res.data);
      })
      .catch(reject);
  });
};

export const voteUpCountAPost = (data) => {
  return new Promise<IPostModel[]>((resolve, reject) => {
    API.put('v1/post-service/posts/post/up', data)
      .then((res) => {
        resolve(res.data);
      })
      .catch(reject);
  });
};

export const voteDownCountAPost = (data) => {
  return new Promise<IPostModel[]>((resolve, reject) => {
    API.put('v1/post-service/posts/post/down', data)
      .then((res) => {
        resolve(res.data);
      })
      .catch(reject);
  });
};

export const getRelyOfComment = (data) => {
  return new Promise<IPostModel[]>((resolve, reject) => {
    API.post('v1/post-service/comments/get-reply-by-commentId', data)
      .then((res) => {
        resolve(res.data);
      })
      .catch(reject);
  });
};

export const createRelyOfComment = (data) => {
  return new Promise<IPostModel[]>((resolve, reject) => {
    API.post('v1/post-service/comments/reply', data)
      .then((res) => {
        resolve(res.data);
      })
      .catch(reject);
  });
};

export const donatePost = (data) => {
  return new Promise<any>((resolve, reject) => {
    API.post('/v1/user-service/users/user-post-donation', data)
      .then((res) => {
        resolve(res.data);
      })
      .catch(reject);
  });
};
export const donateUser = (data) => {
  return new Promise<any>((resolve, reject) => {
    API.post('/v1/user-service/users/donatation', data)
      .then((res) => {
        resolve(res.data);
      })
      .catch(reject);
  });
};

export const requestFeartureDonate = (data) => {
  return new Promise<any>((resolve, reject) => {
    API.post('/v1/user-service/users/create-donate-receiver-request', data)
      .then((res) => {
        resolve(res.data);
      })
      .catch(reject);
  });
};

export const totalReciveDonate = () => {
  return new Promise<any>((resolve, reject) => {
    API.get('/v1/post-service/donatepost/detail-donate')
      .then((res) => {
        resolve(res.data);
      })
      .catch(reject);
  });
};
