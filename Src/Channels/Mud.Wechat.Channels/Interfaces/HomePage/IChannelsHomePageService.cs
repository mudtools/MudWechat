// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Channels.DataModels.HomePage;

namespace Mud.Wechat.Channels;

/// <summary>
/// 微信小店 / 视频号（channels 生态）「主页管理」域 SDK（14 端点：主页商品排序 / 主页背景图 / 精选展示位 / 店铺主页商品分类）。
/// </summary>
/// <remarks>
/// <para>
/// <b>域范围（四组）</b>：① 主页商品排序（<c>/channels/ec/store/window/product/*</c>，4 端点）；
/// ② 主页背景图（<c>/channels/ec/basics/homepage/background/*</c>，4 端点）；③ 精选展示位
/// （<c>/channels/ec/basics/homepage/banner/*</c>，4 端点）；④ 店铺主页商品分类
/// （<c>/channels/ec/store/classification/*</c>，2 端点）。
/// </para>
/// <para>
/// <b>与「视频号橱窗」严格区分（同族不同前缀，设计方案 v1 §4.3）</b>：本域 <c>/channels/ec/store/window/*</c>
/// 是<b>微信小店店铺主页</b>对用户展示的商品；视频号个人橱窗走 <c>/channels/ec/window/product/*</c>
/// （见 <see cref="IChannelsWindowService"/>），官方文档亦分属两套 API 目录——两组端点各自照官方原文落位，
/// 不得合并、不得互相替代。
/// </para>
/// <para>
/// <b>审核流语义</b>：背景图与精选展示位为「提交申请 → 审核 → 生效」模型，查询接口同时返回生效内容与
/// 流程中的申请；「撤销申请」不影响已生效内容，「清空」接口才同时移除生效内容。
/// </para>
/// <para>
/// MUD005 已知接受风险：微信小店官方契约强制令牌走 Query 参数（<c>access_token</c>），
/// 无法改用 Header；库内遥测与异常消息的 URL 已由组件 <c>SensitiveUrlRedactor</c> 与
/// <c>WechatChannelsException</c> 构造期脱敏处置。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "HomePage", TokenManage = nameof(IChannelsAppManager))]
[Token(TokenType = ChannelsTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IChannelsHomePageService
{
    /// <summary>
    /// 获取主页展示商品列表（<c>getstorewindowproductlist</c>，仅返回上架中的商品）。
    /// </summary>
    /// <param name="request">查询请求（<c>page_size</c> 必填，默认 10 且不超过 30；<c>next_key</c> 翻页上下文）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>主页展示商品列表（<c>products</c> 内含 <c>product_id</c>/<c>is_set_hide</c>/<c>is_set_top</c>）与翻页信息（<c>next_key</c>/<c>total_num</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/homepage/storewindow/api_getstorewindowproductlist"/></para>
    /// <para>
    /// 官方业务限制：①仅返回<b>上架中</b>的商品；②隐藏商品只在主页不可见，并不代表下架；
    /// ③专属错误码 <c>10020050</c>（无权限调用）/ <c>10020051</c>（参数有误）。
    /// </para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>129</c>；不支持云调用；仅「微信小店」账号类型可调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/store/window/product/list/get")]
    Task<ChannelsGetStoreWindowProductListResponse> GetStoreWindowProductListAsync(
        [Body] ChannelsGetStoreWindowProductListRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 重新排序主页展示商品（<c>reorderstorewindowproduct</c>）。
    /// </summary>
    /// <param name="request">排序请求（<c>product_id</c>/<c>index_num</c> 必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/homepage/storewindow/api_reorderstorewindowproduct"/></para>
    /// <para>官方业务限制：<c>index_num</c> 为商品重新排序后的新序号，<b>最大移动步长为 500</b>；本接口无特殊注意事项。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>129</c>；不支持云调用；仅「微信小店」账号类型可调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/store/window/product/reorder")]
    Task<ChannelsResponse> ReorderStoreWindowProductAsync(
        [Body] ChannelsReorderStoreWindowProductRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 隐藏小店主页商品（<c>hidestorewindowproduct</c>，设置隐藏或取消隐藏）。
    /// </summary>
    /// <param name="request">隐藏请求（<c>product_id</c>/<c>is_set_hide</c> 必填，1=隐藏 0=取消隐藏）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/homepage/storewindow/api_hidestorewindowproduct"/></para>
    /// <para>
    /// 官方业务限制：①隐藏后的商品<b>不会下架</b>，仍可通过其他渠道购买；②专属错误码 <c>10020052</c>（商品不存在）、
    /// <c>10020298</c>（相似商品经营受限，不允许取消隐藏）、<c>10020341</c>（商品正在参与平台活动，编辑前需退出活动）。
    /// </para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>129</c>；不支持云调用；仅「微信小店」账号类型可调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/store/window/product/hide")]
    Task<ChannelsResponse> HideStoreWindowProductAsync(
        [Body] ChannelsHideStoreWindowProductRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 置顶小店主页商品（<c>settopstorewindowproduct</c>，设置置顶或取消置顶）。
    /// </summary>
    /// <param name="request">置顶请求（<c>product_id</c>/<c>is_set_top</c> 必填，1=置顶 0=取消置顶）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/homepage/storewindow/api_settopstorewindowproduct"/></para>
    /// <para>官方业务限制：本接口无特殊注意事项；专属错误码 <c>10020050</c>（无权限）/ <c>10020051</c>（参数有误）。</para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>129</c>；不支持云调用；仅「微信小店」账号类型可调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/store/window/product/settop")]
    Task<ChannelsResponse> SetTopStoreWindowProductAsync(
        [Body] ChannelsSetTopStoreWindowProductRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 提交主页背景图申请（<c>submitbackgroundapply</c>，审核通过后背景图展示在店铺主页顶部，按 16:9 比例）。
    /// </summary>
    /// <param name="request">申请请求（<c>img_url</c> 必填，须用「上传图片」接口 resp_type=1 返回的 img_url）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>申请编号（<c>apply_id</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/homepage/background/api_submitbackgroundapply"/></para>
    /// <para>
    /// 官方业务限制：①仅支持上传<b>一张</b>背景图，按 16:9 比例展示在主页顶部；②图片支持 bmp、jpg/jpeg、png、svg、webp
    /// 格式，上传大小上限 10MB；③图片须用「上传图片」接口（resp_type=1）获取，不接受其他格式的图片 url
    /// （前缀 <c>mmecimage.cn/p/</c> 的已转换 url 可直接提交）；④已有流程中的申请时不可重复提交
    /// （<c>268549841</c>）。
    /// </para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>129</c>、<c>131</c>、<c>192</c>；不支持云调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/basics/homepage/background/apply/submit")]
    Task<ChannelsHomepageApplyResponse> SubmitBackgroundApplyAsync(
        [Body] ChannelsSubmitHomepageBackgroundApplyRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询背景图（<c>getbackground</c>，返回当前生效背景图与流程中的申请）。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>当前生效背景图（<c>img_url</c>）与最近一次流程中的申请（<c>apply</c>，已生效/已撤销的申请不返回）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/homepage/background/api_getbackground"/></para>
    /// <para>
    /// 官方契约：<b>POST</b> + 空请求体（官方参数表无 Body 参数）。
    /// 官方业务限制：专属错误码 <c>268549676</c>（既无生效中的主页背景图，也无流程中的申请）、
    /// <c>268555516</c>（无权限，仅供微信小店使用）。
    /// </para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>129</c>、<c>131</c>、<c>192</c>；不支持云调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/basics/homepage/background/get")]
    Task<ChannelsGetHomepageBackgroundResponse> GetBackgroundAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 清空主页背景图并撤销流程中的申请（<c>removebackground</c>）。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/homepage/background/api_removebackground"/></para>
    /// <para>
    /// 官方契约：<b>POST</b> + 空请求体（官方参数表无 Body 参数）。
    /// 官方业务限制：本接口<b>同时移除已生效内容并撤销审核中的申请</b>；若仅撤销申请而不影响已生效背景图，
    /// 应改用 <see cref="CancelBackgroundApplyAsync"/>。
    /// </para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>129</c>、<c>131</c>、<c>192</c>；不支持云调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/basics/homepage/background/remove")]
    Task<ChannelsResponse> RemoveBackgroundAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 撤销主页背景图申请（<c>cancelbackgroundapply</c>，仅撤销审核中的申请，不影响已生效背景图）。
    /// </summary>
    /// <param name="request">撤销请求（<c>apply_id</c> 必填，申请编号）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/homepage/background/api_cancelbackgroundapply"/></para>
    /// <para>
    /// 官方业务限制：①仅能撤销<b>流程中（审核中）</b>的申请（<c>268549848</c>），不影响当前已生效的背景图；
    /// ②专属错误码 <c>268549835</c>（未能找到申请）/ <c>268549854</c>（申请编号为空）。
    /// </para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>129</c>、<c>131</c>、<c>192</c>；不支持云调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/basics/homepage/background/apply/cancel")]
    Task<ChannelsResponse> CancelBackgroundApplyAsync(
        [Body] ChannelsHomepageApplyIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 提交精选展示位申请（<c>submitbannerapply</c>，审核通过后展示在店铺主页）。
    /// </summary>
    /// <param name="request">申请请求（<c>banner</c> 模块必填：展示位列表最多 5 个（小图模式最少 3 个）、<c>scale</c> 展示样式必填、模块标题选填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>申请编号（<c>apply_id</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/homepage/banner/api_submitbannerapply"/></para>
    /// <para>
    /// 官方业务限制：①展示位图片同样须用「上传图片」接口（resp_type=1）获取，不接受其他格式 url；
    /// ②<b>type=商品/视频号/公众号三选一，只能设置对应类型的参数</b>（错配报参数错误）；③展示位标题长度受
    /// <c>scale</c> 约束（小图模式 ≤12 字、大图模式 ≤40 字，大图模式可设 ≤100 字描述，小图模式描述必须为空）；
    /// ④已有流程中的申请时不可重复提交（<c>268549841</c>）；⑤视频号 ID / 视频 ID 可在视频号助手首页与
    /// 「动态管理」模块获取，且二者必须匹配（<c>268555694</c>）。
    /// </para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>129</c>、<c>131</c>、<c>192</c>；不支持云调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/basics/homepage/banner/apply/submit")]
    Task<ChannelsHomepageApplyResponse> SubmitBannerApplyAsync(
        [Body] ChannelsSubmitBannerApplyRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询精选展示位（<c>getbanner</c>，返回当前生效展示位与流程中的申请）。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>当前生效展示位（<c>banner</c>）与最近一次流程中的申请（<c>apply</c>，不返回已生效或已撤销的申请）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/homepage/banner/api_getbanner"/></para>
    /// <para>
    /// 官方契约：<b>POST</b> + 空请求体（官方参数表无 Body 参数）。
    /// 官方业务限制：专属错误码 <c>268549675</c>（既无生效中的精选展示位，也无流程中的申请）、
    /// <c>268555516</c>（无权限，仅供微信小店使用）。
    /// </para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>129</c>、<c>131</c>、<c>192</c>；不支持云调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/basics/homepage/banner/get")]
    Task<ChannelsGetBannerResponse> GetBannerAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 清空精选展示位并撤销流程中的申请（<c>removebanner</c>）。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/homepage/banner/api_removebanner"/></para>
    /// <para>
    /// 官方契约：<b>POST</b> + 空请求体（官方参数表无 Body 参数）。
    /// 官方业务限制：本接口<b>同时移除已生效展示位内容并撤销审核中的申请</b>；若仅撤销申请而不影响已生效展示位，
    /// 应改用 <see cref="CancelBannerApplyAsync"/>。
    /// </para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>129</c>、<c>131</c>、<c>192</c>；不支持云调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/basics/homepage/banner/remove")]
    Task<ChannelsResponse> RemoveBannerAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 撤销精选展示位申请（<c>cancelbannerapply</c>，仅撤销审核中的申请，不影响已生效展示位）。
    /// </summary>
    /// <param name="request">撤销请求（<c>apply_id</c> 必填，申请编号）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="ChannelsResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/homepage/banner/api_cancelbannerapply"/></para>
    /// <para>
    /// 官方业务限制：①仅能撤销<b>流程中（审核中）</b>的申请（<c>268549848</c>），不影响当前已生效的展示位；
    /// ②专属错误码 <c>268549835</c>（未能找到申请）/ <c>268549854</c>（申请编号为空）。
    /// </para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>129</c>、<c>131</c>、<c>192</c>；不支持云调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/basics/homepage/banner/apply/cancel")]
    Task<ChannelsResponse> CancelBannerApplyAsync(
        [Body] ChannelsHomepageApplyIdRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取在店铺主页展示的商品分类（<c>getclassificationtree</c>，仅分类树，不含分类与商品的关联数据，支持 2 级树结构）。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>分类树（<c>tree</c>：一级/二级分类列表、树 ID、创建/更新时间）与分类树版本号（<c>version</c>）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/homepage/shoptype/api_getclassificationtree"/></para>
    /// <para>
    /// 官方契约：<b>POST</b> + 空请求体（官方参数表无 Body 参数）。
    /// 官方业务限制：①仅支持 2 级树结构，且<b>不含分类与商品的关联数据</b>（关联商品用
    /// <see cref="GetClassificationProductListAsync"/>）；②专属错误码 <c>10020241</c>（该接口仅支持微信小店调用，请升级后使用）。
    /// </para>
    /// <para>第三方平台代调用所属权限集 id 为 <c>129</c>；不支持云调用；仅「微信小店」账号类型可调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/store/classification/tree/get")]
    Task<ChannelsGetClassificationTreeResponse> GetClassificationTreeAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取分类关联的商品ID列表（<c>getclassificationproductlist</c>）。
    /// </summary>
    /// <param name="request">查询请求（<c>req</c> 对象必填：<c>level_1_id</c>/<c>level_2_id</c>/<c>page_context</c>/<c>page_size</c> 均必填，分类 ID 经分类树接口获取）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>分类关联的商品 ID 列表（<c>product_ids</c>）与翻页上下文（<c>page_context</c>，为空表示已到最后一页）。</returns>
    /// <remarks>
    /// <para>官方文档：<see href="https://developers.weixin.qq.com/doc/store/shop/API/homepage/shoptype/api_getclassificationproductlist"/></para>
    /// <para>
    /// 官方业务限制：①<b>一个分类最多关联 500 个商品</b>；②本接口<b>不支持第三方平台代调用</b>（官方适用范围表未放行）；
    /// ③请求体为嵌套形态 <c>{"req":{…}}</c>（官方参数表原文，照抄建模）。
    /// </para>
    /// <para>不支持云调用。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/channels/ec/store/classification/tree/product/get")]
    Task<ChannelsGetClassificationProductListResponse> GetClassificationProductListAsync(
        [Body] ChannelsGetClassificationProductListRequest request,
        CancellationToken cancellationToken = default);
}
