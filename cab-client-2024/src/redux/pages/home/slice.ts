import { createSlice, ActionReducerMapBuilder } from '@reduxjs/toolkit';
import persistReducer from 'redux-persist/es/persistReducer';
import storage from 'redux-persist/lib/storage';

const initialState = {
  example: 'example',
};

const slice = createSlice({
  name: 'homepage',
  initialState,
  reducers: {
    // add only sync reducer action from this slice file
  },
  extraReducers: (builder: ActionReducerMapBuilder<{}>) => {
    // add async or sync reducer action from other slice
  },
});

// config persist home page state
const persistConfig = {
  key: 'homepage',
  storage,
  whitelist: [],
};
// export selector
export const exampleSelector = (state: any) => state.homepage.example;
export const homepageActions = slice.actions;
// reducer persist
export default persistReducer(persistConfig, slice.reducer);
