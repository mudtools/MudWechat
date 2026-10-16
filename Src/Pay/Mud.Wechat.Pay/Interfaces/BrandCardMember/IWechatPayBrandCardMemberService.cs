// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.DataModels.BrandCardMember;

namespace Mud.Wechat.Pay;

/// <summary>
/// 微信支付「品牌会员卡与会员」域 SDK（15 端点）。
/// </summary>
/// <remarks>
/// <para><b>逐端点契约</b>：每个方法的 remarks 内原样记录官方「请求方式 + 路由 + 支持商户 + 频率限制 + 业务错误码」与
/// 参数表必填口径。本域每支路由官方<b>只印一页</b>（不像营销/商户经营面存在同一路径的【普通商户】与【普通服务商】双页）
/// ⇒ <b>不存在</b>「同路由多页字段表比对」问题，一支路由一方法。</para>
/// <para><b>无 <c>[Token]</c>、走商户签名</b>（守卫 PAY-B1 形态）：形态与 <see cref="IWechatPayTransactionsService"/> 一致，
/// 商户作用域经 <c>IWechatPayMerchantContext.UseMerchant(...)</c> 选择，本域与公众平台令牌面无关。</para>
/// <para><b><c>[AllowAnyStatusCode]</c> 的必要性</b>：APIv3 业务失败是 HTTP 4xx/5xx + <c>{&quot;code&quot;,&quot;message&quot;}</c>，
/// 加上后错误体原样落到 <c>WechatPayResponse.Code</c> / <c>Message</c>，由 <c>WechatPayException.ThrowIfFailed</c> 判错。</para>
/// <para><b>频率限制口径</b>：本域 7 个官方页树<b>全部未印</b>端点级 QPS 数字，只在错误码表给出 <c>429 RATELIMIT_EXCEEDED</c> /
/// <c>FREQUENCY_LIMITED</c> ⇒ 不得凭第三方 SDK 补数，各方法 remarks 内的「未印」结论即取证结果。</para>
/// <para><b>分页口径不统一（照页，不得互套）</b>：本域列表支存在<b>两种</b>整型分页形态
/// —— <c>offset</c>/<c>limit</c>（会员卡模板列表、用户会员卡列表、品牌门店列表）与
/// <c>page_index</c>/<c>page_size</c>（交易连接名片规则列表）；且同为 <c>offset</c>/<c>limit</c>，
/// 会员卡两支官方<b>必填</b>、门店列表支官方<b>选填</b> ⇒ 必填口径也须照页。应答总数字段官方为选填。</para>
/// <para><b><c>brand_id</c> 是本域的「商户作用域参数」</b>：除品牌入驻面（<c>/brand/applyments/*</c>）与免密契约面外，
/// 本域其余每支都把 <c>brand_id</c> 印为<b>必填 query</b> ⇒ 它不是可选过滤条件，缺省即 400；
/// 长度上限官方<b>不统一</b>（名片配置/连接/门店面为 <c>string(20)</c>，会员卡面为 <c>string(32)</c>），照页不做本地归一。</para>
/// <para><b>模板与用户卡是两套生命周期</b>：<c>.../cards</c> 族管的是<b>会员卡模板</b>（创建 / 列表 / 查询 / PATCH 修改 / 作废）， <c>.../user-cards</c> 族管<b>用户持有的卡</b>（查询 / 列表 / PATCH 修改 / 开通结果确认 / 作废 / 按 OPENID 导入） ⇒ 两族的 ID 体系（<c>card_id</c> ↔ <c>user_card_code</c>）与应答字段<b>不同形</b>，不得互相套用类型。</para>
/// <para><b>写动词不统一</b>：修改走 <b>PATCH</b>，作废与确认走 <b>POST .../invalidate</b>、<b>POST .../confirm</b> ⇒ 照页不得合并。</para>
/// <para><b>应答同形族的共用裁定</b>（含 req 标记的签名比对）：模板四支（创建 / 查询 / 修改 / 作废）应答<b>逐字段同形</b>（49 行） ⇒ 共用 <see cref="BrandMemberCardResponse"/>，而<b>列表支是 53 行</b> ⇒ 单建； 用户卡五支（查询 / 修改 / 确认 / 作废 / 导入）同形（28 行）⇒ 共用 <see cref="BrandMemberUserCardResponse"/>， 而<b>列表支 32 行</b> ⇒ 单建。</para>
/// <para><b>请求侧不因应答同形而共用</b>：模板创建（45 行）与 PATCH 修改的<b>必填口径不同</b>、 用户卡的 PATCH 修改 / 确认 / 导入三支互异 ⇒ 各建独立请求类型，只有<b>应答</b>侧才共用。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "BrandCardMember", HttpClient = WechatPayHttpClientNames.TypeName)]
public interface IWechatPayBrandCardMemberService
{
    /// <summary>
    /// 会员卡模板创建。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336970"/>（【普通服务商】）。
    /// </summary>
    /// <param name="request">会员卡模板创建请求体，字段见 <see cref="BrandMemberCardCreateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>会员卡模板创建，见 <see cref="BrandMemberCardResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/brand/partner/card-member/cards</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="BrandMemberCardResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 400 PARAM_ERROR 是否展示会员等级为 true，必填会员初始等级信息 / 400 PARAM_ERROR 是否展示会员等级为 false，不允许填写会员初始等级信息 / 400 PARAM_ERROR 是否支持用户主动入会为 true 时，必须填写会员权益、回调地址、商家法务协议 / 400 PARAM_ERROR 会员码型为跳转商家小程序，必填会员码跳转信息 / 400 PARAM_ERROR 会员码型不是跳转商家小程序，不允许填写会员码跳转信息 / 400 PARAM_ERROR 非储值会员卡，不允许填写储值小程序信息 / 400 PARAM_ERROR 非付费会员卡，不允许填写付费小程序信息 / 400 PARAM_ERROR 付费会员价格必须大于 0 / 400 PARAM_ERROR 商家请求单号重复 / 400 PARAM_ERROR AppID无效或与品牌 ID 没有绑定关系 / 400 PARAM_ERROR 会员中心跳转AppID 无效或与品牌 ID 没有绑定关系 / 400 PARAM_ERROR 会员码跳转AppID无效或与品牌 ID 没有绑定关系 / 400 PARAM_ERROR 积分跳转AppID无效或与品牌 ID 没有绑定关系 / 400 PARAM_ERROR 储值跳转AppID无效或与品牌 ID 没有绑定关系 / 400 PARAM_ERROR 付费跳转AppID无效或与品牌 ID 没有绑定关系 / 400 PARAM_ERROR 请求内容或图片不符合法规，请重试 / 400 INVALID_REQUEST 会员卡模板数量超过限制，每个类型最多创建两个模板 / 400 PARAM_ERROR 卡名称太长，支持最长10个中文字、英文字符、标点 / 400 PARAM_ERROR 卡名称仅支持中英文字符、数字和标点 / 400 INVALID_REQUEST 暂不支持付费或储值的会员卡类型，请修改后重试 / 400 PARAM_ERROR 用户信息通用字段存在重复项 / 400 PARAM_ERROR 用户信息自定义字段存在重复项 / 400 PARAM_ERROR 商户自定义信息最多一项，字段值列表最少1个，最多10个，单个列表限制8个字符 / 400 PARAM_ERROR 用户信息自定义字段中，用户选择的值的字符串非法 / 400 INVALID_REQUEST 商家未开通商家名片会员功能。</para>
    /// </remarks>
    [Post("/v3/brand/partner/card-member/cards")]
    Task<BrandMemberCardResponse> CreateBrandMemberCardAsync(
        [Body] BrandMemberCardCreateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 会员卡模板列表。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336976"/>（【普通服务商】）。
    /// </summary>
    /// <param name="brandId">【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID（灰度期间联系微信支付运营获取），用于标记该会员卡的归属方（官方 query <c>brand_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="offset">【分页开始位置】 该次请求的分页开始位置，从0开始计数，例如offset=10，表示从第11条记录开始返回。（官方 query <c>offset</c>，<b>必填</b>，类型 <c>integer</c>）。</param>
    /// <param name="limit">【分页大小】 分页大小，范围为1~20（官方 query <c>limit</c>，<b>必填</b>，类型 <c>integer</c>）。</param>
    /// <param name="state">【状态】 会员卡状态信息。可选取值：`CARD_EFFECTIVE` 生效中 / `CARD_INVALID` 已失效（官方 query <c>state</c>，选填，类型 <c>string</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>会员卡模板列表，见 <see cref="BrandMemberCardListResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/brand/partner/card-member/cards</c>；query <c>brand_id</c> / <c>offset</c> / <c>limit</c> / <c>state</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>brand_id</c> 与 <c>offset</c> 与 <c>limit</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 400 INVALID_REQUEST 商家未开通商家名片会员功能。</para>
    /// </remarks>
    [Get("/v3/brand/partner/card-member/cards")]
    Task<BrandMemberCardListResponse> QueryBrandMemberCardsAsync(
        [Query("brand_id")] string brandId,
        [Query("offset")] int offset,
        [Query("limit")] int limit,
        [Query("state")] string? state = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 会员卡模板信息。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336974"/>（【普通服务商】）。
    /// </summary>
    /// <param name="cardId">【会员卡模板 ID】 商家 创建会员卡模板 成功后系统返回的会员卡模板ID（官方 path <c>card_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="brandId">【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID（灰度期间联系微信支付运营获取），用于标记该会员卡的归属方（官方 query <c>brand_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>会员卡模板信息，见 <see cref="BrandMemberCardResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/brand/partner/card-member/cards/{card_id}</c>；path <c>card_id</c>，query <c>brand_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>brand_id</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 404 NOT_FOUND 会员卡模板不存在 / 400 INVALID_REQUEST 商家未开通商家名片会员功能。</para>
    /// </remarks>
    [Get("/v3/brand/partner/card-member/cards/{cardId}")]
    Task<BrandMemberCardResponse> QueryBrandMemberCardAsync(
        [Path] string cardId,
        [Query("brand_id")] string brandId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 会员卡模板修改。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336977"/>（【普通服务商】）。
    /// </summary>
    /// <param name="cardId">【会员卡模板 ID】 商家 创建会员卡模板 成功后系统返回的会员卡模板ID（官方 path <c>card_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="request">会员卡模板修改请求体，字段见 <see cref="BrandMemberCardUpdateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>会员卡模板修改，见 <see cref="BrandMemberCardResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>PATCH</b> <c>/v3/brand/partner/card-member/cards/{card_id}</c>；path <c>card_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="BrandMemberCardResponse"/>（官方应答表逐字段建模，与 <c>POST /brand/partner/card-member/cards</c> 应答表<b>逐字段同形且必填口径一致</b> ⇒ 共用同一类型）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 400 PARAM_ERROR 会员中心跳转AppID 无效或与品牌 ID 没有绑定关系 / 400 PARAM_ERROR 会员码跳转AppID无效或与品牌 ID 没有绑定关系 / 400 PARAM_ERROR 积分跳转AppID无效或与品牌 ID 没有绑定关系 / 400 PARAM_ERROR 储值跳转AppID无效或与品牌 ID 没有绑定关系 / 400 PARAM_ERROR 付费跳转AppID无效或与品牌 ID 没有绑定关系 / 400 PARAM_ERROR 请求内容或图片不符合法规，请重试 / 404 NOT_FOUND 会员卡模板不存在 / 400 PARAM_ERROR 会员码型不是跳转商家小程序，不支持修改跳转信息 / 400 PARAM_ERROR 非储值会员卡，不允许填写储值小程序信息 / 400 PARAM_ERROR 非付费会员卡，不允许填写付费小程序信息 / 400 PARAM_ERROR 付费会员价格必须大于 0 / 400 PARAM_ERROR 卡名称太长，支持最长10个中文字、英文字符、标点 / 400 PARAM_ERROR 卡名称仅支持中英文字符、数字和标点 / 400 PARAM_ERROR 卡模版有效期类型和请求的有效期类型不一致 / 400 PARAM_ERROR 绝对有效期卡模板必填有效期信息，不允许填写有效天数信息 / 400 PARAM_ERROR 固定时间范围不允许填写领取后有效天数 / 400 PARAM_ERROR 固定时间范围的开始时间不能大于结束时间 / 400 PARAM_ERROR 相对有效期卡模板必填有效天数信息，不允许填写有效期信息 / 400 PARAM_ERROR 领取后有效天数必须大于0 / 400 PARAM_ERROR 领取后有效天数不能大于30年（10958 天） / 400 PARAM_ERROR 永久有效不允许设置相对有效期 / 400 PARAM_ERROR 永久有效不允许设置固定时间有效的开始时间 / 400 PARAM_ERROR 永久有效不允许设置固定时间有效的结束时间 / 400 PARAM_ERROR 用户信息通用字段存在重复项 / 400 PARAM_ERROR 用户信息自定义字段存在重复项 / 400 PARAM_ERROR 商户自定义信息最多一项，字段值列表最少1个，最多10个，单个列表限制8个字符 / 400 PARAM_ERROR 用户信息自定义字段中，用户选择的值的字符串非法 / 400 PARAM_ERROR 会员中心跳转 AppID 或路径为空 / 400 PARAM_ERROR 会员码跳转 AppID 或路径为空 / 400 PARAM_ERROR 积分跳转 AppID 或路径为空 / 400 PARAM_ERROR 储值跳转 AppID 或路径为空 / 400 PARAM_ERROR 付费跳转 AppID 或路径为空 / 400 PARAM_ERROR 修改后是否支持用户主动入会为 true 时，会员权益、回调地址、商家法务协议必须已填写或在本次补齐 / 400 INVALID_REQUEST 商家未开通商家名片会员功能。</para>
    /// </remarks>
    [Patch("/v3/brand/partner/card-member/cards/{cardId}")]
    Task<BrandMemberCardResponse> UpdateBrandMemberCardAsync(
        [Path] string cardId,
        [Body] BrandMemberCardUpdateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 会员卡模板作废。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336972"/>（【普通服务商】）。
    /// </summary>
    /// <param name="cardId">【会员卡模板ID】 商家 创建会员卡模板 成功后系统返回的会员卡模板ID（官方 path <c>card_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="request">会员卡模板作废请求体，字段见 <see cref="BrandMemberCardInvalidateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>会员卡模板作废，见 <see cref="BrandMemberCardResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/brand/partner/card-member/cards/{card_id}/invalidate</c>；path <c>card_id</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="BrandMemberCardResponse"/>（官方应答表逐字段建模，与 <c>POST /brand/partner/card-member/cards</c> 应答表<b>逐字段同形且必填口径一致</b> ⇒ 共用同一类型）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 404 NOT_FOUND 会员卡模板不存在 / 400 INVALID_REQUEST 商家未开通商家名片会员功能。</para>
    /// </remarks>
    [Post("/v3/brand/partner/card-member/cards/{cardId}/invalidate")]
    Task<BrandMemberCardResponse> InvalidateBrandMemberCardAsync(
        [Path] string cardId,
        [Body] BrandMemberCardInvalidateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 会员卡预授权令牌发起。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336986"/>（【普通服务商】）。
    /// </summary>
    /// <param name="request">会员卡预授权令牌发起请求体，字段见 <see cref="BrandMemberPreAuthTokenRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>会员卡预授权令牌发起，见 <see cref="BrandMemberPreAuthTokenResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/brand/partner/card-member/pre-auth-tokens</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="BrandMemberPreAuthTokenResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 400 INVALID_REQUEST 功能暂未开放 / 400 INVALID_REQUEST 会员卡模板无效 / 400 INVALID_REQUEST 会员卡模板已过期 / 400 INVALID_REQUEST OpenID 不属于品牌的 AppID / 400 INVALID_REQUEST OpenID 无效 / 400 INVALID_REQUEST 用户在一个卡模板下只能有一张有效的会员卡 / 400 INVALID_REQUEST 商家未开通商家名片会员功能。</para>
    /// </remarks>
    [Post("/v3/brand/partner/card-member/pre-auth-tokens")]
    Task<BrandMemberPreAuthTokenResponse> CreateBrandMemberPreAuthTokenAsync(
        [Body] BrandMemberPreAuthTokenRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 用户会员卡信息。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336980"/>（【普通服务商】）。
    /// </summary>
    /// <param name="userCardCode">【会员卡code】 会员在一个会员卡模板下的唯一标识，用户领取会员卡后获得的code。只能填写数字／英文／半角标点。平台支持2种会员卡code分配类型：（1）SYSTEM_ALLOCATE 微信支付系统分配，用户领取会员卡时从微信系统分配24位数字作为会员code；（2）MERCHANT_ALLOCATE 商家分配，商家同步会员开通结果时传入，用户开卡成功或失败都以商家传入的code作为会员卡code。（官方 path <c>user_card_code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="brandId">【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID（灰度期间联系微信支付运营获取），用于标记该会员卡的归属方（官方 query <c>brand_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="cardId">【会员卡模板 ID】 商家 创建会员卡模板 成功后系统返回的会员卡模板ID（官方 query <c>card_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="openid">【用户标识】 用户在品牌商家会员卡模板AppID下的唯一标识，获取方式详见 参数说明。（官方 query <c>openid</c>，<b>必填</b>，类型 <c>string(128)</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>用户会员卡信息，见 <see cref="BrandMemberUserCardResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/brand/partner/card-member/user-cards/{user_card_code}</c>；path <c>user_card_code</c>，query <c>brand_id</c> / <c>card_id</c> / <c>openid</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>brand_id</c> 与 <c>card_id</c> 与 <c>openid</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 400 INVALID_REQUEST OpenID 不属于品牌的 AppID / 400 INVALID_REQUEST OpenID 无效 / 404 NOT_FOUND 用户会员卡不存在 / 404 NOT_FOUND 会员卡模板不存在 / 400 INVALID_REQUEST 商家未开通商家名片会员功能。</para>
    /// </remarks>
    [Get("/v3/brand/partner/card-member/user-cards/{userCardCode}")]
    Task<BrandMemberUserCardResponse> QueryBrandMemberUserCardAsync(
        [Path] string userCardCode,
        [Query("brand_id")] string brandId,
        [Query("card_id")] string cardId,
        [Query("openid")] string openid,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 用户会员卡列表。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336984"/>（【普通服务商】）。
    /// </summary>
    /// <param name="brandId">【品牌ID】 商家进驻微信支付品牌商家后获得的品牌ID（灰度期间联系微信支付运营获取），用于标记该会员卡的归属方（官方 query <c>brand_id</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="openid">【用户标识】 用户在品牌商家会员卡模板AppID下的唯一标识，获取方式详见 参数说明。（官方 query <c>openid</c>，<b>必填</b>，类型 <c>string(128)</c>）。</param>
    /// <param name="offset">【分页开始位置】 该次请求的分页开始位置，从0开始计数，例如offset=10，表示从第11条记录开始返回。（官方 query <c>offset</c>，<b>必填</b>，类型 <c>integer</c>）。</param>
    /// <param name="limit">【分页大小】 分页大小，范围为1~20（官方 query <c>limit</c>，<b>必填</b>，类型 <c>integer</c>）。</param>
    /// <param name="userCardState">【用户会员卡状态】 用户当前的卡状态。可选取值：`UNACTIVATED` 用户已领卡，但还未激活 / `EFFECTIVE` 用户的会员卡可正常使用 / `EXPIRED` 用户的会员卡已过期 / `INVALID` 用户的会员卡已失效（官方 query <c>user_card_state</c>，选填，类型 <c>string</c>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>用户会员卡列表，见 <see cref="BrandMemberUserCardListResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>GET</b> <c>/v3/brand/partner/card-member/user-cards</c>；query <c>brand_id</c> / <c>openid</c> / <c>offset</c> / <c>limit</c> / <c>user_card_state</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b><c>brand_id</c> 与 <c>openid</c> 与 <c>offset</c> 与 <c>limit</c> 是这支官方必填 query</b> ⇒ 声明式签名须显式给出，缺省即 400。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 400 INVALID_REQUEST OpenID 不属于品牌的 AppID / 400 INVALID_REQUEST OpenID 无效 / 400 INVALID_REQUEST 商家未开通商家名片会员功能。</para>
    /// </remarks>
    [Get("/v3/brand/partner/card-member/user-cards")]
    Task<BrandMemberUserCardListResponse> QueryBrandMemberUserCardsAsync(
        [Query("brand_id")] string brandId,
        [Query("openid")] string openid,
        [Query("offset")] int offset,
        [Query("limit")] int limit,
        [Query("user_card_state")] string? userCardState = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 用户会员卡信息修改。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336985"/>（【普通服务商】）。
    /// </summary>
    /// <param name="userCardCode">【会员卡code】 会员在一个会员卡模板下的唯一标识，用户领取会员卡后获得的code。只能填写数字／英文／半角标点。平台支持2种会员卡code分配类型：（1）SYSTEM_ALLOCATE 微信支付系统分配，用户领取会员卡时从微信系统分配24位数字作为会员code；（2）MERCHANT_ALLOCATE 商家分配，商家同步会员开通结果时传入，用户开卡成功或失败都以商家传入的code作为会员卡code。（官方 path <c>user_card_code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="request">用户会员卡信息修改请求体，字段见 <see cref="BrandMemberUserCardUpdateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>用户会员卡信息修改，见 <see cref="BrandMemberUserCardResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>PATCH</b> <c>/v3/brand/partner/card-member/user-cards/{user_card_code}</c>；path <c>user_card_code</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="BrandMemberUserCardResponse"/>（官方应答表逐字段建模，与 <c>GET /brand/partner/card-member/user-cards/{user_card_code}</c> 应答表<b>逐字段同形且必填口径一致</b> ⇒ 共用同一类型）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 400 PARAM_ERROR 请求内容或图片不符合法规，请重试 / 400 INVALID_REQUEST OpenID 不属于品牌的 AppID / 400 INVALID_REQUEST OpenID 无效 / 404 NOT_FOUND 用户会员卡不存在 / 400 INVALID_REQUEST 用户卡状态不正确，只能操作已激活的会员卡 / 400 PARAM_ERROR 卡模版有效期类型和请求的有效期类型不一致 / 400 PARAM_ERROR 绝对有效期卡模板必填有效期信息，不允许填写有效天数信息 / 400 PARAM_ERROR 固定时间范围不允许填写领取后有效天数 / 400 PARAM_ERROR 固定时间范围的开始时间不能大于结束时间 / 400 PARAM_ERROR 相对有效期卡模板必填有效天数信息，不允许填写有效期信息 / 400 PARAM_ERROR 领取后有效天数必须大于0 / 400 PARAM_ERROR 领取后有效天数不能大于30年（10958 天） / 400 PARAM_ERROR 永久有效不允许设置相对有效期 / 400 PARAM_ERROR 永久有效不允许设置固定时间有效的开始时间 / 400 PARAM_ERROR 永久有效不允许设置固定时间有效的结束时间 / 400 PARAM_ERROR 用户信息通用字段存在重复项 / 400 PARAM_ERROR 用户信息自定义字段存在重复项 / 400 PARAM_ERROR 商户自定义信息最多一项，字段值列表最少1个，最多10个，单个列表限制8个字符 / 400 PARAM_ERROR 用户信息自定义字段中，用户选择的值的字符串非法 / 400 PARAM_ERROR 请求的用户信息通用字段未定义在卡模版中 / 400 PARAM_ERROR 用户信息通用字段中，生日格式不正确 / 400 PARAM_ERROR 用户信息通用字段中，邮箱格式不正确 / 400 PARAM_ERROR 用户信息通用字段中，性别格式不正确 / 400 PARAM_ERROR 请求的用户信息自定义字段未定义在卡模版中 / 404 NOT_FOUND 会员卡模板不存在 / 400 INVALID_REQUEST 商家未开通商家名片会员功能。</para>
    /// </remarks>
    [Patch("/v3/brand/partner/card-member/user-cards/{userCardCode}")]
    Task<BrandMemberUserCardResponse> UpdateBrandMemberUserCardAsync(
        [Path] string userCardCode,
        [Body] BrandMemberUserCardUpdateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 会员开通结果同步。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336979"/>（【普通服务商】）。
    /// </summary>
    /// <param name="userCardCode">【会员卡code】 会员在一个会员卡模板下的唯一标识，用户领取会员卡后获得的code。只能填写数字／英文／半角标点。平台支持2种会员卡code分配类型：（1）SYSTEM_ALLOCATE 微信支付系统分配，用户领取会员卡时从微信系统分配24位数字作为会员code；（2）MERCHANT_ALLOCATE 商家分配，商家同步会员开通结果时传入，用户开卡成功或失败都以商家传入的code作为会员卡code。（官方 path <c>user_card_code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="request">会员开通结果同步请求体，字段见 <see cref="BrandMemberUserCardConfirmRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>会员开通结果同步，见 <see cref="BrandMemberUserCardResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/brand/partner/card-member/user-cards/{user_card_code}/confirm</c>；path <c>user_card_code</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="BrandMemberUserCardResponse"/>（官方应答表逐字段建模，与 <c>GET /brand/partner/card-member/user-cards/{user_card_code}</c> 应答表<b>逐字段同形且必填口径一致</b> ⇒ 共用同一类型）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 400 PARAM_ERROR 用户卡code 无效，只能包含数字、英文、半角标点 / 400 PARAM_ERROR 请求内容或图片不符合法规，请重试 / 404 NOT_FOUND 用户不存在未激活的会员卡 / 400 INVALID_REQUEST 该用户卡 code 已存在 / 400 INVALID_REQUEST 会员卡号分配方式为系统分配，不允许修改会员卡号 / 400 INVALID_REQUEST 用户卡状态不正确，只能操作已激活的会员卡 / 400 INVALID_REQUEST OpenID 不属于品牌的 AppID / 400 INVALID_REQUEST OpenID 无效 / 404 NOT_FOUND 会员卡模板不存在 / 400 PARAM_ERROR 卡模版有效期类型和请求的有效期类型不一致 / 400 PARAM_ERROR 绝对有效期卡模板必填有效期信息，不允许填写有效天数信息 / 400 PARAM_ERROR 固定时间范围不允许填写领取后有效天数 / 400 PARAM_ERROR 固定时间范围的开始时间不能大于结束时间 / 400 PARAM_ERROR 相对有效期卡模板必填有效天数信息，不允许填写有效期信息 / 400 PARAM_ERROR 领取后有效天数必须大于0 / 400 PARAM_ERROR 领取后有效天数不能大于30年（10958 天） / 400 PARAM_ERROR 永久有效不允许设置相对有效期 / 400 PARAM_ERROR 永久有效不允许设置固定时间有效的开始时间 / 400 PARAM_ERROR 永久有效不允许设置固定时间有效的结束时间 / 400 PARAM_ERROR 用户信息通用字段存在重复项 / 400 PARAM_ERROR 用户信息自定义字段存在重复项 / 400 PARAM_ERROR 商户自定义信息最多一项，字段值列表最少1个，最多10个，单个列表限制8个字符 / 400 PARAM_ERROR 用户信息自定义字段中，用户选择的值的字符串非法 / 400 PARAM_ERROR 请求的用户信息通用字段未定义在卡模版中 / 400 PARAM_ERROR 用户信息通用字段中，生日格式不正确 / 400 PARAM_ERROR 用户信息通用字段中，邮箱格式不正确 / 400 PARAM_ERROR 用户信息通用字段中，性别格式不正确 / 400 PARAM_ERROR 请求的用户信息自定义字段未定义在卡模版中。</para>
    /// </remarks>
    [Post("/v3/brand/partner/card-member/user-cards/{userCardCode}/confirm")]
    Task<BrandMemberUserCardResponse> ConfirmBrandMemberUserCardAsync(
        [Path] string userCardCode,
        [Body] BrandMemberUserCardConfirmRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 用户会员卡作废。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336983"/>（【普通服务商】）。
    /// </summary>
    /// <param name="userCardCode">【会员卡code】 会员在一个会员卡模板下的唯一标识，用户领取会员卡后获得的code。只能填写数字／英文／半角标点。平台支持2种会员卡code分配类型：（1）SYSTEM_ALLOCATE 微信支付系统分配，用户领取会员卡时从微信系统分配24位数字作为会员code；（2）MERCHANT_ALLOCATE 商家分配，商家同步会员开通结果时传入，用户开卡成功或失败都以商家传入的code作为会员卡code。（官方 path <c>user_card_code</c>，<b>必填</b>，类型 <c>string(32)</c>）。</param>
    /// <param name="request">用户会员卡作废请求体，字段见 <see cref="BrandMemberUserCardInvalidateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>用户会员卡作废，见 <see cref="BrandMemberUserCardResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/brand/partner/card-member/user-cards/{user_card_code}/invalidate</c>；path <c>user_card_code</c>。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="BrandMemberUserCardResponse"/>（官方应答表逐字段建模，与 <c>GET /brand/partner/card-member/user-cards/{user_card_code}</c> 应答表<b>逐字段同形且必填口径一致</b> ⇒ 共用同一类型）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 400 INVALID_REQUEST 用户卡状态不正确，只能作废待激活或已生效的会员卡 / 400 INVALID_REQUEST OpenID 不属于品牌的 AppID / 400 INVALID_REQUEST OpenID 无效 / 404 NOT_FOUND 用户会员卡不存在 / 404 NOT_FOUND 会员卡模板不存在 / 400 PARAM_ERROR OpenID有误 / 400 INVALID_REQUEST 商家未开通商家名片会员功能。</para>
    /// </remarks>
    [Post("/v3/brand/partner/card-member/user-cards/{userCardCode}/invalidate")]
    Task<BrandMemberUserCardResponse> InvalidateBrandMemberUserCardAsync(
        [Path] string userCardCode,
        [Body] BrandMemberUserCardInvalidateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 按 OPENID 导入用户会员卡。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336981"/>（【普通服务商】）。
    /// </summary>
    /// <param name="request">按 OPENID 导入用户会员卡请求体，字段见 <see cref="BrandMemberUserCardImportRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>按 OPENID 导入用户会员卡，见 <see cref="BrandMemberUserCardResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/brand/partner/card-member/user-cards/import-by-openid</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>（官方原样）：（2）若当前接口的频率限制无法满足业务需要，请联系微信支付技术支持评估。。</para>
    /// <para><b>应答</b>见 <see cref="BrandMemberUserCardResponse"/>（官方应答表逐字段建模，与 <c>GET /brand/partner/card-member/user-cards/{user_card_code}</c> 应答表<b>逐字段同形且必填口径一致</b> ⇒ 共用同一类型）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 400 PARAM_ERROR 用户卡code 无效，只能包含数字、英文、半角标点 / 400 PARAM_ERROR 固定时间范围必填领取时间 / 400 INVALID_REQUEST 无法导入已过期的会员卡 / 400 PARAM_ERROR 请求内容或图片不符合法规，请重试 / 400 INVALID_REQUEST OpenID 不属于品牌的 AppID / 400 INVALID_REQUEST OpenID 无效 / 400 INVALID_REQUEST 用户已有待激活的会员卡 / 400 INVALID_REQUEST 用户已有已生效的会员卡 / 400 INVALID_REQUEST 用户主动删除了该会员卡，无法重新导入 / 400 INVALID_REQUEST 该用户卡 code 已存在 / 400 INVALID_REQUEST 会员卡模板无效 / 400 INVALID_REQUEST 商家未开通商家名片会员功能。</para>
    /// </remarks>
    [Post("/v3/brand/partner/card-member/user-cards/import-by-openid")]
    Task<BrandMemberUserCardResponse> ImportBrandMemberUserCardAsync(
        [Body] BrandMemberUserCardImportRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 用户动态信息创建。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015336987"/>（【普通服务商】）。
    /// </summary>
    /// <param name="request">用户动态信息创建请求体，字段见 <see cref="BrandMemberUserFeedCreateRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>用户动态信息创建，见 <see cref="BrandMemberUserFeedCreateResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/brand/partner/card-member/user-feeds</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="BrandMemberUserFeedCreateResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 403 NO_AUTH 商家没有权限使用该动态信息模板 / 400 INVALID_REQUEST 当前用户非会员 / 400 INVALID_REQUEST 会员卡模板无效 / 400 PARAM_ERROR cell不合法 / 400 ALREADY_EXISTS 该用户动态已存在 / 400 INVALID_REQUEST 品牌未开通名片会员 / 400 INVALID_REQUEST openid无效。</para>
    /// </remarks>
    [Post("/v3/brand/partner/card-member/user-feeds")]
    Task<BrandMemberUserFeedCreateResponse> CreateBrandMemberUserFeedAsync(
        [Body] BrandMemberUserFeedCreateRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 积分兑券结果同步。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015897268"/>（【普通服务商】）。
    /// </summary>
    /// <param name="request">积分兑券结果同步请求体，字段见 <see cref="BrandMemberPointsCouponConfirmRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>积分兑券结果同步，见 <see cref="BrandMemberPointsCouponConfirmResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/brand/partner/card-member/user-points/exchange-coupon/confirm</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="BrandMemberPointsCouponConfirmResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 400 INVALID_REQUEST OpenID 不属于品牌的 AppID / 400 INVALID_REQUEST OpenID 无效 / 404 NOT_FOUND 用户会员卡不存在 / 404 NOT_FOUND 积分兑券记录不存在。</para>
    /// </remarks>
    [Post("/v3/brand/partner/card-member/user-points/exchange-coupon/confirm")]
    Task<BrandMemberPointsCouponConfirmResponse> ConfirmBrandMemberPointsCouponExchangeAsync(
        [Body] BrandMemberPointsCouponConfirmRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 积分余额同步。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v3/partner/4015897280"/>（【普通服务商】）。
    /// </summary>
    /// <param name="request">积分余额同步请求体，字段见 <see cref="BrandMemberPointsSyncRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>积分余额同步，见 <see cref="BrandMemberPointsSyncResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/v3/brand/partner/card-member/user-points/sync</c>；无 path / query 参数。</para>
    /// <para><b>支持商户</b>【普通服务商】。</para>
    /// <para><b>频率限制</b>：官方页<b>未印</b>端点级数字（只在错误码表里给出 <c>429 RATELIMIT_EXCEEDED</c> / <c>FREQUENCY_LIMITED</c>）⇒ 不得凭第三方 SDK 补数。</para>
    /// <para><b>应答</b>见 <see cref="BrandMemberPointsSyncResponse"/>（官方应答表逐字段建模）。</para>
    /// <para><b>业务错误码</b>：400 PARAM_ERROR 参数错误 / 400 INVALID_REQUEST HTTP 请求不符合微信支付 APIv3 接口规则 / 401 SIGN_ERROR 验证不通过 / 500 SYSTEM_ERROR 系统异常，请稍后重试 / 400 INVALID_REQUEST OpenID 不属于品牌的 AppID / 400 INVALID_REQUEST OpenID 无效 / 404 NOT_FOUND 用户会员卡不存在。</para>
    /// </remarks>
    [Post("/v3/brand/partner/card-member/user-points/sync")]
    Task<BrandMemberPointsSyncResponse> SyncBrandMemberPointsAsync(
        [Body] BrandMemberPointsSyncRequest request,
        CancellationToken cancellationToken = default);
}
