using System;
using OptimizingLastMile.Entites.Enums;

namespace OptimizingLastMile.Models.Commons;

public class Errors
{
    public static class Auth
    {
        public static ErrorObject UsernameNotExist()
        {
            return new ErrorObject(ErrorCodeEnum.USERNAME_NOT_EXIST.ToString(),
                "Username not exist");
        }

        public static ErrorObject PasswordIncorrect()
        {
            return new(ErrorCodeEnum.PASSWORD_NOT_CORRECT.ToString(),
                "Password incorrect");
        }

        public static ErrorObject AccountIsDisable()
        {
            return new(ErrorCodeEnum.ACCOUNT_IS_DISABLE.ToString(),
                "Account is inactive");
        }

        public static ErrorObject AccountIsReject()
        {
            return new(ErrorCodeEnum.ACCOUNT_REJECTED.ToString(),
                "Account rejected");
        }

        public static ErrorObject UsernameAlreadyExist()
        {
            return new(ErrorCodeEnum.ACCOUNT_ALREADY_EXIST.ToString(),
                "Account already exist");
        }

        public static ErrorObject RoleNotAllowRegisterByUsername()
        {
            return new(ErrorCodeEnum.ROLE_NOT_ALLOW_RESGISTER_BY_USERNAME.ToString(),
                "Not allow this role to register account by username");
        }
    }

    public static class AccountProfile
    {
        public static ErrorObject InvalidBirthDay()
        {
            return new(ErrorCodeEnum.INVALID_BIRTHDAY.ToString(),
                "Invalid birth day");
        }

        public static ErrorObject NotDeactiveDriver()
        {
            return new(ErrorCodeEnum.DRIVER_HAVE_ORDER.ToString(),
                "Driver still have order need to be deliver. Cannot deactive right now");
        }

        public static ErrorObject NotDeactiveManager()
        {
            return new(ErrorCodeEnum.MANAGER_HAVE_ORDER.ToString(),
                "Manager still have order shipping. Cannot deactive right now");
        }

        public static ErrorObject NotDeactiveCustomer()
        {
            return new(ErrorCodeEnum.CUSTOMER_HAVE_ORDER.ToString(),
                "Customer still have order need to process. Cannot deactive right now");
        }
    }

    public static class Order
    {
        public static ErrorObject InvalidOwner()
        {
            return new(ErrorCodeEnum.INVALID_OWNER.ToString(),
                "Invalid owner");
        }

        public static ErrorObject InvalidDriver()
        {
            return new(ErrorCodeEnum.INVALID_DRIVER.ToString(),
                "Invalid driver");
        }

        public static ErrorObject NotAllowUpdateOrderForCurrentStatus()
        {
            return new(ErrorCodeEnum.NOT_ALLOW_UPDATE_FOR_CURRENT_STATUS.ToString(),
                "Update order fail because current status of order not allow to update");
        }

        public static ErrorObject NotAllowDeleteOrderForCurrentStatus()
        {
            return new(ErrorCodeEnum.NOT_ALLOW_DELETE_FOR_CURRENT_STATUS.ToString(),
                "Delete order fail because current status of order not allow to delete");
        }

        public static ErrorObject NotAllowFeedback()
        {
            return new(ErrorCodeEnum.NOT_ALLOW_FEEDBACK.ToString(),
                "Not allow to feedback order when it not yet Delivered");
        }

        public static ErrorObject RoleNotAllowUpdateThisStatus(OrderStatusEnum status)
        {
            return new(ErrorCodeEnum.ROLE_NOT_ALLOW_UPDATE_THIS_STATUS.ToString(),
                $"This role doesn't allow update {status} status");
        }

        public static ErrorObject InvalidUpdateOrderStatus()
        {
            return new(ErrorCodeEnum.INVALID_UPDATE_ORDER_STATUS.ToString(),
                "Invalid status to update. You not allow to update this status or missing other step before can update this status");
        }
    }

    public static class Common
    {
        public static ErrorObject MethodNotAllow()
        {
            return new(ErrorCodeEnum.METHOD_NOT_ALLOW.ToString(),
                "Method not allow this perform");
        }

        public static ErrorObject UnknownError()
        {
            return new(ErrorCodeEnum.UNKNOWN_ERROR.ToString(),
                "Unknown error try again, or contact to admin");
        }

        public static ErrorObject FeatureLock()
        {
            return new(ErrorCodeEnum.FEATURE_LOCK.ToString(),
                "Feature is lock by Admin, contact Admin to enable this feature");
        }
    }

    public static class OTP
    {
        public static ErrorObject TooManyRequest()
        {
            return new(ErrorCodeEnum.TOO_MANY_REQUEST_OTP.ToString(),
                "OTP max attempts reached request");
        }

        public static ErrorObject TooManyVerify()
        {
            return new(ErrorCodeEnum.TOO_MANY_VERIFY_OTP.ToString(),
                "OTP max attempts reached verify");
        }

        public static ErrorObject OtpTimeOut()
        {
            return new(ErrorCodeEnum.VERIFY_OTP_TIME_OUT.ToString(),
                "The verification code has timed out");
        }

        public static ErrorObject VerificationCodeIncorrect()
        {
            return new(ErrorCodeEnum.VERIFICATION_CODE_INCORRECT.ToString(),
                "The verification code is incorrect");
        }
    }

    public static class Firebase
    {
        public static ErrorObject InvalidTokenFirebase()
        {
            return new(ErrorCodeEnum.INVALID_TOKEN_FIREBASE.ToString(),
                "Invalid Id token firebase");
        }
    }
}