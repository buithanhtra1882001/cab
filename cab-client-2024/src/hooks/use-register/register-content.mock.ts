import { IRegisterPageContent } from '../../models';

export const registerContent: IRegisterPageContent = {
  title: 'Đăng kí tài khoản',
  btnRegister: 'Đăng kí',
  checkBoxHasAccount: 'Tôi đã có tài khoản',
  infoRegister:
    'Bằng việc đăng kí tài khoản, bạn đã đồng ý với Điều khoản sử dụng và dã đọc Chính sách quyền riêng tư của chúng tôi.',

  labelEmail: 'Email',
  labelFullName: 'Tên của bạn',
  labelUsername: 'Tên tài khoản',
  labelPassword: 'Mật khẩu',
  labelConfirmPassword: 'Xác nhận mật khẩu',
  labelCodeRef: 'Mã giới thiệu',

  errorRequiredEmail: 'Email không được để trống',
  errorRequiredFullName: 'Họ tên không được để trống',
  errorRequiredUsername: 'Tên tài khoản không được để trống',
  errorRequirePassword: 'Mật khẩu ít nhất 6 ký tự',
  errorRequirePasswordMinLength: 'Mật không được để trống',
  errorRequireConfirmPassword: 'Mật không được để trống',
  errorRequireConfirmPasswordMinLength: 'Mật khẩu ít nhất 6 ký tự',
  errorRequireConfirmPasswordNotMatchValidation: 'Mật khẩu không khớp',
  errorRequireCodeRef: 'Mã giới thiệu không được để trống',

  placeholderEmail: '',
  placeholderFullName: '',
  placeholderUsername: '',
  placeholderPassword: '',
  placeholderConfirmPassword: '',
  placeholderCodeRef: '',

  defaultValueEmail: '',
  defaultValueFullName: '',
  defaultValueUsername: '',
  defaultValuePassword: '',
  defaultValueConfirmPassword: '',
  defaultValueCodeRef: '',
};
