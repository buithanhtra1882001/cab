import { BehaviorSubject } from 'rxjs';
import { LangService } from '../../configuration';
import { IModal } from './modal.type';
import en from '../../languages/en.json';
import vi from '../../languages/vi.json';
import zh from '../../languages/zh.json';
import { getUuid } from '../../utils';

export const LANGUAGE_CORE_UI_MODAL_NAMESPACE = 'core-ui-modal';
type ModalMemoryCache = {
  [placeholder: string]: IModal | null;
};

class ModalServiceFactory {
  private static _instance: ModalServiceFactory;

  private _modalMemoryService$ = new BehaviorSubject<ModalMemoryCache | null>(null);

  private constructor() {
    LangService.instance().addResourceBundle('vi', LANGUAGE_CORE_UI_MODAL_NAMESPACE, vi);
    LangService.instance().addResourceBundle('en', LANGUAGE_CORE_UI_MODAL_NAMESPACE, en);
    LangService.instance().addResourceBundle('zh', LANGUAGE_CORE_UI_MODAL_NAMESPACE, zh);
  }

  public static instance(): ModalServiceFactory {
    if (!this._instance) {
      this._instance = new ModalServiceFactory();
    }

    return this._instance;
  }

  public showModal(placeholderUuid: string, modal: IModal) {
    let newModalMemory = this._modalMemoryService$.getValue();
    newModalMemory = { ...newModalMemory };
    newModalMemory[placeholderUuid] = {
      type: 'normal',
      ...modal,
      uuid: getUuid(),
      placeholderUuid,
    };
    this._modalMemoryService$.next({ ...newModalMemory });
  }

  public removeModal(placeholderUuid: string, modalUuid: string) {
    const newModalMemory = this._modalMemoryService$.getValue();
    const modal = newModalMemory?.[placeholderUuid];
    if (modal && modal.uuid === modalUuid) {
      newModalMemory[placeholderUuid] = null;
      this._modalMemoryService$.next({ ...newModalMemory });
    }
  }

  public removeModalPlaceholder(placeholderUuid: string) {
    const newModalMemory = this._modalMemoryService$.getValue();
    if (newModalMemory) {
      delete newModalMemory[placeholderUuid];
      this._modalMemoryService$.next({ ...newModalMemory });
    }
  }

  public ModalServiceSubject() {
    return this._modalMemoryService$;
  }
}

export const ModalService = ModalServiceFactory.instance();
