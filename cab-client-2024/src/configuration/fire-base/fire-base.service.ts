import firebase from 'firebase/compat/app';
import { fireBaseConfig } from './fire-base.config';
import { getFirestore } from 'firebase/firestore/lite';
import { getAuth, signInWithPopup, GoogleAuthProvider } from "firebase/auth";

class FireBaseServiceFactory {
  private static _instance: FireBaseServiceFactory;

  public static instance(): FireBaseServiceFactory {
    if (!this._instance) {
      this._instance = new FireBaseServiceFactory();
    }

    return this._instance;
  }

  private constructor() {
    this.init();
  }

  private _firebaseApp!: firebase.app.App;

  private async init(): Promise<void> {
    if (typeof window !== 'undefined') {
      if (!firebase.app.length){
        this._firebaseApp = firebase.initializeApp(fireBaseConfig);
      }
    }
  }

  public getFireStore = () => {
    return getFirestore(this._firebaseApp);
  };

  public getFirebaseGoogleAuthProvider = () => {
    return new firebase.auth.GoogleAuthProvider();
  };

  public getFirebaseAuth = () => {
    return this._firebaseApp.auth();
  };
}

export const FireBaseService = FireBaseServiceFactory.instance();