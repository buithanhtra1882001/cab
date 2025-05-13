import { createSlice, PayloadAction, createAsyncThunk } from '@reduxjs/toolkit';
import { IUserProfile } from '../../../models';
import { userService } from '../../../services/user.service';

interface AuthState {
  isAuthenticated: boolean;
  profile: IUserProfile | null;
}

const initialState: AuthState = {
  isAuthenticated: false,
  profile: null,
};

export const fetchProfile = createAsyncThunk('auth/getProfile', async (args, { rejectWithValue }) => {
  try {
    const response = await userService.profile();
    return response.data;
  } catch (error) {
    return rejectWithValue(error);
  }
});

const slice = createSlice({
  name: 'auth',
  initialState,
  reducers: {
    setUserProfile: (state: AuthState, { payload }: PayloadAction<IUserProfile>) => {
      state.profile = payload;
    },

    updateIsAuthenticated: (state: AuthState, { payload }: PayloadAction<boolean>) => {
      state.isAuthenticated = payload;
    },
  },

  extraReducers: (builder) => {
    builder.addCase(fetchProfile.fulfilled, (state: AuthState, { payload }: PayloadAction<IUserProfile>) => {
      // state.loading = false;
      state.profile = { ...payload };
    });
    builder.addCase(fetchProfile.rejected, (state: AuthState) => {
      state.profile = null;
    });
  },
});

export const authActions = slice.actions;
export default slice.reducer;
