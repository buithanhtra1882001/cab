using CabUserService.Constants;
using CabUserService.Controllers.Base;
using CabUserService.Cqrs.Requests.Commands;
using CabUserService.Cqrs.Requests.Queries;
using CabUserService.Models.Dtos;
using CabUserService.Services.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace CabUserService.Controllers
{
    public class UsersController : ApiController<UsersController>
    {
        #region Properties

        private readonly IProfileService _profileService;
        private readonly IUserCategoryService _userCategoryService;
        private readonly IServiceProvider _serviceProvider;
        private readonly IBalanceService _balanceService;
        private readonly ICategoryService _categoryService;
        private readonly IUserService _userService;
        private readonly IDonateReceiverRequestService _donateReceiverRequestService;
        private readonly IWithdrawalRequestService _withdrawalRequestService;
        private readonly IMediator _mediator;

        #endregion Properties

        #region Constructor

        public UsersController(ILogger<UsersController> logger,
            IProfileService profileService,
            IServiceProvider serviceProvider,
            IBalanceService balanceService,
            IUserCategoryService userCategoryService,
            ICategoryService categoryService,
            IUserService userService,
            IDonateReceiverRequestService donateReceiverRequestService,
            IMediator mediator,
            IWithdrawalRequestService withdrawalRequestService)
            : base(logger)
        {
            _serviceProvider = serviceProvider;
            _balanceService = balanceService;
            _profileService = profileService;
            _userCategoryService = userCategoryService;
            _categoryService = categoryService;
            _userService = userService;
            _donateReceiverRequestService = donateReceiverRequestService;
            _mediator = mediator;
            _withdrawalRequestService = withdrawalRequestService;
        }

        #endregion Constructor

        #region Method

        /// <summary>
        /// Get an user informations.
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="401">Authentication failed</response>
        /// <response code="403">Authorization failed</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(FullUserInformationResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUser([Required] Guid auid, Guid? id)
        {
            var response = new Response();
            var profile = await _profileService.UserGetProfileAsync(auid, id);

            if (profile is null)
            {
                response.StatusCode = StatusCodes.Status404NotFound;
                response.Message = "User not found";
            }
            else
            {
                response.StatusCode = StatusCodes.Status200OK;
                response.Data = profile;
            }

            return Ok(response);

        }


        /// <summary>
        /// Get list users information by list userid 
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="401">Authentication failed</response>
        /// <response code="403">Authorization failed</response>
        [HttpPost("ids")]
        [ProducesResponseType(typeof(IEnumerable<FullUserInformationResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetListUser([Required] IEnumerable<Guid> auids)
        {
            if (auids == null || !auids.Any())
            {
                return BadRequest("Invalid data ids");
            }
            List<PublicUserInformationResponse> profiles = await _profileService.GetListUserProfileAsync(auids);

            return Ok(profiles);
        }

        /// <summary>
        /// Find an user by properties
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="401">Authentication is failed</response>
        /// <response code="403">Authorization is failed</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<UserFindByUserNameResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> FindUser([Required] Guid auid, string username)
        {
            var httpMessageResponse = new HttpMessageResponse();
            var profile = await _profileService.UserGetListAsync(auid, username);

            if (profile is null)
            {
                httpMessageResponse.Message = "Profile not found";

                return NotFound(httpMessageResponse);
            }

            return Ok(profile);
        }

        /// <summary>
        /// Update user profile
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="401">Authentication failed</response>
        /// <response code="403">Authorization failed</response>
        [HttpPost("")]
        [ProducesResponseType(typeof(HttpMessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateProfile(
            [Required] Guid auid,
            [FromBody] UserCreateUpdateRequest userUpdateRequest)
        {
            var httpMessageResponse = new HttpMessageResponse();

            await _profileService.UserCreateOrUpdateProfileAsync(auid, userUpdateRequest);

            httpMessageResponse.Message = "Update profile successful";
            httpMessageResponse.isSuccess = true;

            return Ok(httpMessageResponse);
        }

        /// <summary>
        /// Upload profile image
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="401">Authentication failed</response>
        /// <response code="403">Authorization failed</response>
        [HttpPost("Upload"), DisableRequestSizeLimit]
        [ProducesResponseType(typeof(IEnumerable<ImageUploadResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadImage([Required] Guid auid)
        {
            var formCollection = await Request.ReadFormAsync();
            var files = formCollection.Files;
            var type = formCollection["type"];

            var fileService = _serviceProvider.GetRequiredService<IFileService>();
            var result = await fileService.UploadUserImagesAsync(BearerToken, type, auid, files);

            return (result is null) ? BadRequest() : Ok(result);
        }

        /// <summary>
        /// Upload profile image
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="401">Authentication failed</response>
        /// <response code="403">Authorization failed</response>
        [HttpPost("upload-avatar"), DisableRequestSizeLimit]
        [RequestSizeLimit(bytes: 5_000_000)] // 5Mb 
        [ProducesResponseType(typeof(IEnumerable<ImageUploadResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadAvatarIO([Required] Guid auid, IFormFile file)
        {
            var type = "avatar";
            var fileService = _serviceProvider.GetRequiredService<IFileService>();

            var result = await fileService.UploadAvatarUserImagesAsync(BearerToken, type, auid, file);
            var message = string.Empty;

            if (result is not null)
                message = await _profileService.UpdateAvatarAsync(auid, result.FirstOrDefault().Id.ToString(), result.FirstOrDefault().Url);

            return (!string.IsNullOrEmpty(message) || result == null) ? BadRequest(message) : Ok(result);
        }

        /// <summary>
        /// User donation return transaction log id
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="401">Authentication failed</response>
        /// <response code="403">Authorization failed</response>
        [HttpPost("user-donation")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UserDonateAsync(
            [Required] Guid auid,
            [FromBody] UserDonationRequest request)
        {
            var result = await _balanceService.UserDonataionAsync(auid, request, null);

            return (result is null) ? BadRequest() : Ok(result);
        }

        /// <summary>
        /// User post donation, returns transaction log id
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="401">Authentication failed</response>
        /// <response code="403">Authorization failed</response>
        [HttpPost("user-post-donation")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UserPostDonateAsync(
            [Required] Guid auid,
            [FromBody] UserPostDonationRequest request)
        {
            var result = await _balanceService.UserDonataionAsync(auid, request, request.PostId);

            return (result is null) ? BadRequest() : Ok(result);
        }

        #region DonateReceiverRequest

        /// <summary>
        /// Create request to receive donation
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="401">Authentication failed</response>
        /// <response code="403">Authorization failed</response>
        [HttpPost("create-donate-receiver-request")]
        [ProducesResponseType(typeof(DonateReceiverRequestResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateDonateReceiverRequest(
            [Required] Guid auid,
            [FromBody] DonateReceiverRequestDto request)
        {
            var result = await _donateReceiverRequestService.CreateRequestAsync(auid, request);

            return (result is null) ? BadRequest() : Ok(result);
        }

        /// <summary>
        /// Get donate receiver request of an user
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="401">Authentication failed</response>
        /// <response code="403">Authorization failed</response>
        [HttpGet("get-donate-receiver-request")]
        [ProducesResponseType(typeof(DonateReceiverRequestResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDonateReceiverAsync([Required] Guid auid)
        {
            var result = await _donateReceiverRequestService.GetRequestAsync(auid);

            return (result is null) ? BadRequest() : Ok(result);
        }

        /// <summary>
        /// Update request to receive donation
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="401">Authentication failed</response>
        /// <response code="403">Authorization failed</response>
        [HttpPost("update-donate-receiver-request")]
        [ProducesResponseType(typeof(DonateReceiverRequestResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateDonateReceiverRequest(
            [Required] Guid auid,
            [FromBody] DonateReceiverRequestDto requestDto)
        {
            var result = await _donateReceiverRequestService.UpdateRequestAsync(auid, requestDto);

            return (result is null) ? BadRequest() : Ok(result);
        }

        /// <summary>
        /// Approve request to receive donation
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="401">Authentication failed</response>
        /// <response code="403">Authorization failed</response>
        [HttpPost("approve-donate-receiver-request")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ApproveReceiveDonationRequest(
            [Required] Guid auid, Guid userToBeApprovedId)
        {
            // auid here should be admin id -> need to move this to adminController later
            await _donateReceiverRequestService.ApproveRequest(auid, userToBeApprovedId);

            return Ok();
        }

        #endregion DonateReceiverRequest

        #region WithdrawalRequest

        /// <summary>
        /// Create request to withdrwal money
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="401">Authentication failed</response>
        /// <response code="403">Authorization failed</response>
        [HttpPost("create-withdrawal-request")]
        [ProducesResponseType(typeof(WithdrawalRequestResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateWithdrawalRequest(
            [Required] Guid auid,
            [FromBody] WithdrawalRequestDto request)
        {
            var result = await _withdrawalRequestService.CreateRequestAsync(auid, request);

            return (result is null) ? BadRequest() : Ok(result);
        }

        /// <summary>
        /// Get pending withdrawal request of an user (one pending request at a time only)
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="401">Authentication failed</response>
        /// <response code="403">Authorization failed</response>
        [HttpGet("get-pending-withdrawal-request")]
        [ProducesResponseType(typeof(WithdrawalRequestResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetPendingRequestAsync([Required] Guid auid)
        {
            var result = await _withdrawalRequestService.GetPendingRequestAsync(auid);

            return (result is null) ? BadRequest() : Ok(result);
        }

        /// <summary>
        /// Get all withdrawal requests
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="401">Authentication failed</response>
        /// <response code="403">Authorization failed</response>
        [HttpGet("get-all-withdrawal-requests")]
        [ProducesResponseType(typeof(List<WithdrawalRequestResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllWithdrawalRequests([Required] Guid auid)
        {
            // auid here should be admin id -> need to move this to adminController later
            var result = await _withdrawalRequestService.GetAllRequestsAsync();

            return (result is null) ? BadRequest() : Ok(result);
        }

        /// <summary>
        /// Approve request withdraw coin, returns transaction log id
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="401">Authentication failed</response>
        /// <response code="403">Authorization failed</response>
        [HttpPost("approve-withdrawal-request")]
        [ProducesResponseType(typeof(Guid?), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ApproveWithdrawalRequest([Required] Guid auid, [Required] Guid requestId)
        {
            // auid here should be admin id -> need to move this to adminController later
            var result = await _withdrawalRequestService.ApproveRequest(requestId);

            return (result is null) ? BadRequest() : Ok(result);
        }

        #endregion WithdrawalRequest

        /// <summary>
        /// Get livestream donate URL param
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="401">Authentication failed</response>
        /// <response code="403">Authorization failed</response>
        [HttpGet("donate-url-param")]
        [ProducesResponseType(typeof(Guid), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDonateURL([Required] Guid auid)
        {
            var result = await _balanceService.GetDonateURLParam(auid);

            return (result is null) ? BadRequest() : Ok(result);
        }

        /// <summary>
        /// GetTotalDonate
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="401">Authentication failed</response>
        /// <response code="403">Authorization failed</response>
        [HttpPost("total-donate")]
        [ProducesResponseType(typeof(decimal), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTotalDonate(
            [Required] Guid auid,
            [FromBody] TotalDonateRequest request)
        {
            var result = await _balanceService.GetTotalDonateAsync(auid, request);

            return Ok(result);
        }

        /// <summary>
        /// GetTopDonator
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="401">Authentication failed</response>
        /// <response code="403">Authorization failed</response>
        [HttpPost("top-donator")]
        [ProducesResponseType(typeof(List<DonatorResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTopDonator(
            [Required] Guid auid,
            [FromBody] TotalDonateRequest request)
        {
            var result = await _balanceService.GetTopDonatorAsync(auid, request);

            return Ok(result);
        }

        /// <summary>
        /// User get balance logs
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="401">Authentication failed</response>
        /// <response code="403">Authorization failed</response>
        [HttpPost("balance-logs")]
        [ProducesResponseType(typeof(PagingResponse<UserBalanceLogDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetBalanceLogs(
            [Required] Guid auid,
            [FromBody] GetUserBalanceLogRequest request)
        {
            var result = await _balanceService.GetUserBalanceLogAsync(auid, request);

            return Ok(result);
        }

        /// <summary>
        /// User get transaction logs
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="401">Authentication failed</response>
        /// <response code="403">Authorization failed</response>
        [HttpPost("transactions")]
        [ProducesResponseType(typeof(PagingResponse<UserTransactionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTransactionLogs(
            [Required] Guid auid,
            [FromBody] GetUserTransactionRequest request)
        {
            var result = await _balanceService.GetUserTransactionLogAsync(auid, request);

            return Ok(result);
        }

        /// <summary>
        /// User get transaction log by id
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="401">Authentication failed</response>
        /// <response code="403">Authorization failed</response>
        [HttpPost("transaction/{id}")]
        [ProducesResponseType(typeof(UserTransactionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTransactionLogById(
            [Required] Guid auid,
            [FromBody] GetUserTransactionByIdRequest request)
        {
            var result = await _balanceService.GetUserTransactionByIdAsync(auid, request);

            return (result is null) ? BadRequest() : Ok(result);
        }

        /// <summary>
        /// Follow Category
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="401">Authentication failed</response>
        /// <response code="403">Authorization failed</response>
        [HttpPost("categories/add-follow"), DisableRequestSizeLimit]
        [ProducesResponseType(typeof(HttpMessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddFollowCategories([FromBody] FollowCategoriesRequest categoriesResquest)
        {
            var httpMessageResponse = new HttpMessageResponse();
            var result = await _userCategoryService.FollowCategoriesAsync(categoriesResquest.UserId, categoriesResquest.CategoryIds);
            if (string.IsNullOrEmpty(result))
            {
                httpMessageResponse.Message = "Follow successful";
                return Ok(httpMessageResponse);
            }
            else
            {
                httpMessageResponse.Message = result;
                return BadRequest(httpMessageResponse);
            }
        }
        /// <summary>
        /// Get User Follow Categories
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="401">Authentication failed</response>
        /// <response code="403">Authorization failed</response>
        [HttpGet("user/followed-categories")]
        [ProducesResponseType(typeof(List<CategoryResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserFollowedCategories([Required] Guid auid)
        {
            var httpMessageResponse = new HttpMessageResponse();
            var result = await _userCategoryService.GetUserFollowedCategoriesAsync(auid);
            if (result != null && result.Any())
            {
                return Ok(result);
            }
            else
            {
                httpMessageResponse.Message = "No data found";
                return NotFound(httpMessageResponse);
            }

        }
        /// <summary>
        /// Follower user
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="401">Authentication failed</response>
        /// <response code="403">Authorization failed</response>
        [HttpPost("add-follower"), DisableRequestSizeLimit]
        [ProducesResponseType(typeof(IEnumerable<ImageUploadResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddFollower([Required] Guid auid, string userFollowId)
        {

            var httpMessageResponse = new HttpMessageResponse();

            string result = await _profileService.AddFollowerAsync(auid, userFollowId);

            if (string.IsNullOrEmpty(result))
            {
                httpMessageResponse.isSuccess = true;
                httpMessageResponse.Message = "Follow successful";
            }
            else
            {
                httpMessageResponse.isSuccess = false;
                httpMessageResponse.Message = result;
            }

            return Ok(httpMessageResponse);
        }

        /// <summary>
        /// Following user
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="401">Authentication failed</response>
        /// <response code="403">Authorization failed</response>
        //[HttpPost("add-following"), DisableRequestSizeLimit]
        //[ProducesResponseType(typeof(IEnumerable<ImageUploadResponse>), StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status401Unauthorized)]
        //[ProducesResponseType(StatusCodes.Status403Forbidden)]
        //[ProducesResponseType(StatusCodes.Status500InternalServerError)]
        //public async Task<IActionResult> AddFollowing([Required] Guid auid, FollowRequest followRequest)
        //{

        //    var httpMessageResponse = new HttpMessageResponse();

        //    string result = await _profileService.AddFollowingAsync(followRequest.UserId, followRequest.FollowId.ToString());

        //    if (string.IsNullOrEmpty(result))
        //    {
        //        httpMessageResponse.isSuccess = true;
        //        httpMessageResponse.Message = "Following successful";
        //    }
        //    else
        //    {
        //        httpMessageResponse.isSuccess = false;
        //        httpMessageResponse.Message = result;
        //    }

        //    return Ok(httpMessageResponse);
        //}

        /// <summary>
        /// Get detail follower of user
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="401">Authentication failed</response>
        /// <response code="403">Authorization failed</response>
        [HttpGet("get-detail-follower"), DisableRequestSizeLimit]
        [ProducesResponseType(typeof(IEnumerable<ImageUploadResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDetailFollower(Guid id)
        {
            Response result = await _profileService.GetFollowListAsync(id, FOLLOW_TYPE.FOLLOWER);
            return Ok(result);
        }

        // <summary>
        /// Get detail following of user
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="401">Authentication failed</response>
        /// <response code="403">Authorization failed</response>
        [HttpGet("get-detail-following"), DisableRequestSizeLimit]
        [ProducesResponseType(typeof(IEnumerable<ImageUploadResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetDetailFollowing(Guid id)
        {
            Response result = await _profileService.GetFollowListAsync(id, FOLLOW_TYPE.FOLLOWING);
            return Ok(result);
        }

        /// <summary>
        /// Total follow of user
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="401">Authentication failed</response>
        /// <response code="403">Authorization failed</response>
        [HttpGet("get-total-follow"), DisableRequestSizeLimit]
        [ProducesResponseType(typeof(IEnumerable<ImageUploadResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFollow(Guid id)
        {
            TotalFollowResponse result = await _profileService.GetTotalFollowAsync(id);

            return Ok(result);
        }

        /// <summary>
        /// Get All Categories
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="401">Authentication failed</response>
        /// <response code="403">Authorization failed</response>
        [HttpGet("get-categories")]
        [ProducesResponseType(typeof(List<CategoryResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllCategories()
        {
            var httpMessageResponse = new HttpMessageResponse();
            var result = await _categoryService.GetAllCategoriesAsync();
            if (result != null && result.Any())
            {
                return Ok(result);
            }
            else
            {
                httpMessageResponse.Message = "No data found";
                return NotFound(httpMessageResponse);
            }
        }

        /// <summary>
        /// User request Creator
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="401">Authentication failed</response>
        /// <response code="403">Authorization failed</response>
        [HttpPost("request-creator")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> RequestCreator(
            [Required] Guid auid)
        {
            var result = await _userService.RequestCreatorAsync(auid);

            return Ok(result);
        }

        /// <summary>
        /// Admin confirm creator
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="401">Authentication failed</response>
        /// <response code="403">Authorization failed</response>
        [HttpPost("confirm-creator")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ConfirmCreator(
            [Required] Guid userId)
        {
            var result = await _userService.ConfirmCreatorAsync(userId);

            return Ok(result);
        }

        /// <summary>
        /// Lấy 5 bản ghi đề xuất bạn bè
        /// </summary>
        /// <param name="auid"></param>
        /// <returns></returns>
        [HttpGet("get-friend-suggestion")]
        [ProducesResponseType(typeof(List<UserRequestFriendDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetRequestFriend([Required] Guid auid)
        {
            var result = await _userService.GetRequestFriendAsync(auid);
            return Ok(result);
        }

        /// <summary>
        /// Creator hide or show donation content
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="401">Authentication is failed</response>
        /// <response code="403">Authorization is failed</response>
        [HttpPost("donatation/hide-or-show-content")]
        [ProducesResponseType(typeof(PagingResponse<UserTransactionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreatorHideOrShowDonationContent(
            [Required] Guid auid,
            [Required] Guid transactionId)
        {
            var result = await _balanceService.CreatorHideOrShowDonationContentAsync(auid, transactionId);

            return Ok(result);
        }

        /// <summary>
        /// get request creator
        /// </summary>
        /// <param name="auid"></param>
        /// <returns></returns>
        [HttpGet("get-creator-suggestion")]
        [ProducesResponseType(typeof(List<CreatorResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetRequestCreator([Required] Guid auid)
        {
            var result = await _userService.GetRequestCreatorAsync(BearerToken, auid);

            return Ok(result);
        }

        /// <summary>
        /// Add friend request
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="401">Authentication failed</response>
        /// <response code="403">Authorization failed</response>
        [HttpPost("add-friend-request"), DisableRequestSizeLimit]
        [ProducesResponseType(typeof(HttpMessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddFriendRequestAsync([Required] Guid auid, RequestFriendRequest request)
        {

            var httpMessageResponse = new HttpMessageResponse();

            string result = await _userService.AddFriendRequestAsync(auid, request);

            if (string.IsNullOrEmpty(result))
            {
                httpMessageResponse.isSuccess = true;
                httpMessageResponse.Message = result;
            }
            else
            {
                httpMessageResponse.isSuccess = false;
                httpMessageResponse.Message = result;
            }

            return Ok(httpMessageResponse);
        }

        /// <summary>
        /// get friend request
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="401">Authentication failed</response>
        /// <response code="403">Authorization failed</response>
        [HttpPost("get-friend-request"), DisableRequestSizeLimit]
        [ProducesResponseType(typeof(List<RequestFriendResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFriendRequestAsync([Required] Guid auid)
        {

            var result = await _userService.GetFriendRequestAsync(auid);
            return Ok(result);
        }

        /// <summary>
        /// Add friend
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="401">Authentication failed</response>
        /// <response code="403">Authorization failed</response>
        [HttpPost("add-friend"), DisableRequestSizeLimit]
        [ProducesResponseType(typeof(HttpMessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddFriendAsync([Required] Guid auid, AcceptFriendRequest request)
        {

            var httpMessageResponse = new HttpMessageResponse();

            string result = await _userService.AddFriendAsync(auid, request);

            if (string.IsNullOrEmpty(result))
            {
                httpMessageResponse.isSuccess = true;
                httpMessageResponse.Message = result;
            }
            else
            {
                httpMessageResponse.isSuccess = false;
                httpMessageResponse.Message = result;
            }

            return Ok(httpMessageResponse);
        }

        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="401">Authentication failed</response>
        /// <response code="403">Authorization failed</response>

        [HttpPost("profile/view"), DisableRequestSizeLimit]
        [ProducesResponseType(typeof(HttpMessageResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ViewUserProfileAsync([Required] Guid auid, [Required] Guid profileUserId)
        {
            var httpMessageResponse = new HttpMessageResponse();

            bool result = await _profileService.ViewUserProfileAsync(auid, profileUserId);

            httpMessageResponse.isSuccess = result;

            httpMessageResponse.Message = result ? "View Success" : "View Fail";

            return Ok(httpMessageResponse);
        }

        /// <summary>
        /// Count my profile has view by week, month, year
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="401">Authentication failed</response>
        /// <response code="403">Authorization failed</response>
        [HttpGet("profile/count-view"), DisableRequestSizeLimit]
        [ProducesResponseType(typeof(long), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> StatisticsProfileViewAsync([Required] Guid auid, [Required] IntervalType intervalType)
        {
            long result = await _profileService.StatisticsProfileViewAsync(auid, intervalType);

            return Ok(result);
        }

        /// <summary>
        /// Interaction statistics of user by week, month, year
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="401">Authentication failed</response>
        /// <response code="403">Authorization failed</response>
        [HttpGet("profile/interaction-statistics"), DisableRequestSizeLimit]
        [ProducesResponseType(typeof(InteractionStatisticsResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> InteractionStatisticsAsync([Required] Guid auid, [Required] IntervalType intervalType)
        {
            var result = await _profileService.InteractionStatisticsAsync(auid, intervalType);
            return Ok(result);
        }

        /// <summary>
        /// Get leaderboard at homepage
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="401">Authentication failed</response>
        /// <response code="403">Authorization failed</response>
        [HttpGet("leaderboard"), DisableRequestSizeLimit]
        [ProducesResponseType(typeof(LeaderBoardResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetLeaderBoardAsync()
        {
            var result = await _userService.GetLeaderBoardAsync();
            return Ok(result);
        }

        [HttpPost("get-user-friends")]
        [ProducesResponseType(typeof(PagingResponse<UserFriendResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserFriends(
            [Required] Guid auid,
            [FromBody] GetUserFriendsRequest request)
        {
            request.UserId = auid;
            var result = await _userService.GetUserFriendsAsync(request);
            return Ok(result);
        }

        /// <summary>
        /// Unfriend
        /// </summary>
        /// <response code="200">Unfriend successful</response>
        /// <response code="401">Authentication is failed</response>
        /// <response code="403">Authorization is failed</response>
        [HttpDelete("unfriend/{friendId}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Unfriend([Required] Guid auid, [Required] Guid friendId)
        {
            var httpMessageResponse = new HttpMessageResponse();
            var result = await _userService.UnfriendAsync(auid, friendId);
            if (result is null)
            {
                httpMessageResponse.Message = "unfriend is failed.";
                return NotFound(httpMessageResponse);
            }
            return Ok(result);
        }

        /// <summary>
        /// Unfollower
        /// </summary>
        /// <response code="200">Unfollower successful</response>
        /// <response code="401">Authentication is failed</response>
        /// <response code="403">Authorization is failed</response>
        [HttpDelete("unfollower/{followerId}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Unfollower([Required] Guid auid, [Required] Guid followerId)
        {
            var httpMessageResponse = new HttpMessageResponse();
            var result = await _profileService.UnfollowerAsync(auid, followerId);
            if (result is null)
            {
                httpMessageResponse.Message = "unfollower is failed.";
                return NotFound(httpMessageResponse);
            }
            return Ok(result);
        }

        /// <summary>
        /// Get users send you a message
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="401">Authentication failed</response>
        /// <response code="403">Authorization failed</response>
        [HttpPost("users-message"), DisableRequestSizeLimit]
        [ProducesResponseType(typeof(PagingResponse<UserMessageResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserMessageAsync([Required] Guid auid, GetUserMessagesByUserIdRequest request)
        {
            request.UserId = auid;
            var result = await _userService.GetMessagesByUserIdAsync(request);

            return Ok(result);
        }
        /// <summary>
        /// Get friends is online
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="401">Authentication failed</response>
        /// <response code="403">Authorization failed</response>
        [HttpPost("friend-isonline"), DisableRequestSizeLimit]
        [ProducesResponseType(typeof(PagingResponse<UserOnlineResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetUserFriendOnlineAsync([Required] Guid auid, UserFriendOnlineRequest request)
        {
            request.UserId = auid;
            var result = await _userService.GetUserFriendOnlineAsync(request);

            return Ok(result);
        }
        /// <summary>
        /// Get is user online
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="401">Authentication failed</response>
        /// <response code="403">Authorization failed</response>
        [HttpGet("IsUserOnline/{id}")]
        public async Task<IActionResult> IsUserOnline(Guid id)
        {
            bool isOnline = await _userService.IsUserOnlineAsync(id);
            return Ok(isOnline);
        }       
        /// <summary>
        /// Get content message
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="401">Authentication failed</response>
        /// <response code="403">Authorization failed</response>
        [HttpPost("content-message"), DisableRequestSizeLimit]
        [ProducesResponseType(typeof(PagingStateResponse<MessagesResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetContentMessagesAsync([Required] Guid auid, GetContentMessageRequest request)
        {
            request.CurrentUserId = auid;
            var result = await _userService.GetContentMessagesAsync(request);

            return Ok(result);
        }

        /// <summary>
        /// Upload Cover Image At Profile page
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="401">Authentication failed</response>
        /// <response code="403">Authorization failed</response>
        [HttpPost("upload-cover-image"), DisableRequestSizeLimit]
        [RequestSizeLimit(bytes: 5_000_000)] // 5Mb 
        [ProducesResponseType(typeof(IEnumerable<ImageUploadResponse>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UploadCoverImage([Required] Guid auid, IFormFile file)
        {
            var type = "cover-image";
            var fileService = _serviceProvider.GetRequiredService<IFileService>();

            var result = await fileService.UploadBackgroundUserImagesAsync(BearerToken, type, auid, file);
            var message = string.Empty;

            if (result is not null)
                message = await _profileService.UpdateBackgroundCoverAsync(auid, result.FirstOrDefault().Id.ToString(), result.FirstOrDefault().Url);

            return (!string.IsNullOrEmpty(message) || result == null) ? BadRequest(message) : Ok(result);
        }

        /// <summary>
        /// update profile user
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="401">Authentication failed</response>
        /// <response code="403">Authorization failed</response>
        [HttpPost("update-profile"), DisableRequestSizeLimit]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateProfileAsync([FromBody] UpdateProfileCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        /// <summary>
        /// update profile user
        /// </summary>
        /// <response code="200">Successful</response>
        /// <response code="401">Authentication failed</response>
        /// <response code="403">Authorization failed</response>
        [HttpGet("statistical-user"), DisableRequestSizeLimit]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UserAsync([FromQuery] StatisticalUserQuery query)
        {
            var response = await _mediator.Send(query);
            return Ok(response);
        }

        #endregion
    }
}