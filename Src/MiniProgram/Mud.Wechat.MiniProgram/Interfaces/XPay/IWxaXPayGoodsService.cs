// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「虚拟支付 · 道具管理」域 SDK（5 端点：批量上传道具 / 查询上传任务 /
/// 启动批量发布道具 / 查询发布任务 / 上传媒体文件）。
/// </summary>
/// <remarks>
/// <para>
/// 域统一形态（POST only、不支持云调用、权限集 157、<c>pay_sig</c> 落 Query 的凭据处置、敏感值口径与
/// 全域错误码表）见 <see cref="IWxaXPayCurrencyService"/>，本文件只写本族特有事实。
/// </para>
/// <para>
/// <b>本族是「开发环境 → 现网」的两段式发布</b>：上传只把道具放进开发环境，发布才把它推到现网 ⇒
/// 跳过发布直接让用户买单，官方按 <c>268490008</c>（代币/道具未发布）拒绝。
/// </para>
/// <para>
/// <b>上传与发布两支各占一条任务位</b>：官方 <c>268490012</c> 原文「批量任务运行中，请等待完成后才能再次运行」
/// ⇒ 提交前须先查任务状态，而非并行提交后靠错误码兜。
/// </para>
/// <para><b>本族 5 端点全部只带 <c>pay_sig</c></b>，无用户态签名支（道具是商户侧资产，不属任一用户）。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "XPay", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaXPayGoodsService
{
    /// <summary>
    /// 批量上传道具（<c>POST /xpay/start_upload_goods</c>，不支持云调用，权限集 157）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_start_upload_goods.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>upload_item</c> 逐道具五支全必填 + <c>env</c>），见 <see cref="WxaXPayStartUploadGoodsRequest"/>。</param>
    /// <param name="paySig">支付签名（官方 Query 参数 <c>pay_sig</c>，必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>官方应答体只有 <c>errcode</c> / <c>errmsg</c> ⇒ 直接返回 <see cref="WxaResponse"/>，道具结果须走 <see cref="QueryUploadGoodsAsync"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + <c>pay_sig</c>；端点名里的 <c>start</c> 是语义核心 —— 本调用只<b>提交任务</b>，
    /// 同步成功不等于道具已入库。
    /// </para>
    /// <para>
    /// <b><c>item_url</c> 在本请求里是「原始地址」、在查询结果里是「微信转存后地址」</b>（官方两处说明原文不同）⇒
    /// 对账不得比对二者，道具图以官方转存结果为准。
    /// </para>
    /// <para><b>长度上限官方照录于条目注释</b>（<c>id</c>/<c>name</c> (0,20]、<c>remark</c> (0,1024]、<c>price</c> &gt;0，
    /// 且中文算一个字符）⇒ SDK 不本地校验，超限由官方 <c>268490002</c> 判回。</para>
    /// </remarks>
    [Post("/xpay/start_upload_goods")]
    Task<WxaResponse> StartUploadGoodsAsync(
        [Body] WxaXPayStartUploadGoodsRequest request,
        [Query("pay_sig")] string paySig,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询批量上传道具任务（<c>POST /xpay/query_upload_goods</c>，不支持云调用，权限集 157）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_query_upload_goods.html"/>。
    /// </summary>
    /// <param name="request">请求体（只有 <c>env</c> 一支必填），见 <see cref="WxaXPayQueryUploadGoodsRequest"/>。</param>
    /// <param name="paySig">支付签名（官方 Query 参数 <c>pay_sig</c>，必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>任务总状态 + 逐道具上传结果，见 <see cref="WxaXPayQueryUploadGoodsResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + <c>pay_sig</c>，请求无分页、无筛选 ⇒ <b>本查询返回的是当前环境的全量条目结果</b>，
    /// 不能当增量游标使用。
    /// </para>
    /// <para>
    /// <b>任务级 <c>status</c> 与条目级 <c>upload_status</c> 是两个口径</b>（前者见 <see cref="WxaXPayPublishTaskStatuses"/>
    /// 的取值集，后者 0 上传中 / 1 id 已存在 / 2 上传成功 / 3 上传失败，官方写在说明列、无枚举表故不建常量类）⇒
    /// 只有任务跑完才逐条看结果。
    /// </para>
    /// <para><b><c>upload_status = 1</c>（id 已存在）不是失败</b>：官方把它单列一值 ⇒ 重复提交同 id 时不得当成错误重试。</para>
    /// </remarks>
    [Post("/xpay/query_upload_goods")]
    Task<WxaXPayQueryUploadGoodsResponse> QueryUploadGoodsAsync(
        [Body] WxaXPayQueryUploadGoodsRequest request,
        [Query("pay_sig")] string paySig,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 启动批量发布道具任务（<c>POST /xpay/start_publish_goods</c>，不支持云调用，权限集 157）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_start_publish_goods.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>publish_item</c> 每项只有 <c>id</c> + <c>env</c>），见 <see cref="WxaXPayStartPublishGoodsRequest"/>。</param>
    /// <param name="paySig">支付签名（官方 Query 参数 <c>pay_sig</c>，必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>官方应答体只有 <c>errcode</c> / <c>errmsg</c> ⇒ 直接返回 <see cref="WxaResponse"/>，发布结果须走 <see cref="QueryPublishGoodsAsync"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + <c>pay_sig</c>；发布请求<b>只需道具 id</b>（官方原文「添加到开发环境时传的道具id」）
    /// ⇒ 名称与价格来自上传阶段，本端点不重传、也无法在此修正。
    /// </para>
    /// <para>
    /// <b><c>env</c> 必填且这里指的是签名校验环境</b>（0 现网 / 1 沙箱，与本域其余端点同口径）⇒
    /// 沙箱环境发布的道具不会进入现网可售列表。
    /// </para>
    /// </remarks>
    [Post("/xpay/start_publish_goods")]
    Task<WxaResponse> StartPublishGoodsAsync(
        [Body] WxaXPayStartPublishGoodsRequest request,
        [Query("pay_sig")] string paySig,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询批量发布道具任务（<c>POST /xpay/query_publish_goods</c>，不支持云调用，权限集 157）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_query_publish_goods.html"/>。
    /// </summary>
    /// <param name="request">请求体（只有 <c>env</c> 一支必填），见 <see cref="WxaXPayQueryPublishGoodsRequest"/>。</param>
    /// <param name="paySig">支付签名（官方 Query 参数 <c>pay_sig</c>，必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>任务总状态 + 逐道具发布结果（<c>id</c> + <c>publish_status</c> + <c>errmsg</c>），见 <see cref="WxaXPayQueryPublishGoodsResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + <c>pay_sig</c>；<see cref="WxaXPayPublishTaskStatuses"/> 是本页官方枚举表出处
    /// （0 无任务 / 1 进行中 / 2 失败或部分失败 / 3 成功），<b>只有 2 才需要逐条目读 <c>errmsg</c></b>。
    /// </para>
    /// <para>
    /// <b>发布结果侧没有 <c>name</c>/<c>price</c></b>（与上传结果侧不同）⇒ 不要把两支结果类型共用，
    /// 该差异由守卫 XP5 锁定。
    /// </para>
    /// </remarks>
    [Post("/xpay/query_publish_goods")]
    Task<WxaXPayQueryPublishGoodsResponse> QueryPublishGoodsAsync(
        [Body] WxaXPayQueryPublishGoodsRequest request,
        [Query("pay_sig")] string paySig,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 上传媒体文件（<c>POST /xpay/upload_vp_file</c>，不支持云调用，权限集 157）：为道具图与投诉凭证换取 <c>file_id</c>。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/VirtualPayment/api_upload_vp_file.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>env</c> / <c>base64_img</c> / <c>img_url</c> / <c>file_name</c>），见 <see cref="WxaXPayUploadVpFileRequest"/>。</param>
    /// <param name="paySig">支付签名（官方 Query 参数 <c>pay_sig</c>，必填）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>可供投诉回复引用的 <c>file_id</c>，见 <see cref="WxaXPayUploadVpFileResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + <c>pay_sig</c>，<c>Content-Type</c> 是 <b>JSON 而非 multipart</b> ⇒
    /// 图片走字段承载（<c>base64_img</c> 或 <c>img_url</c>），与本线短剧媒资的分片上传通路形态不同。
    /// </para>
    /// <para>
    /// <b>两支通道的上限不同且官方明示优先后者</b>：<c>base64_img</c> ≤1MB、<c>img_url</c> ≤2MB，
    /// 且 <c>img_url</c> 必须能直接下载（<b>不能是 302 类跳转地址</b>）。
    /// </para>
    /// <para>
    /// <b>参数表的必填标记与「二选一」语义冲突</b>：官方把 <c>base64_img</c> 与 <c>img_url</c> 都标为必填，
    /// 而正文语义是「用哪个传哪个」⇒ 本 SDK 照参数表保留两支可空属性，由宿主按通道赋值，
    /// <b>不</b>把其一并列成必填（该取舍由守卫 XP3 登记）。
    /// </para>
    /// <para><b><c>268490007</c> 是本页最常见的失败形态</b>（图片或文字存在敏感内容）⇒ 道具图与凭证图同受内容审核。</para>
    /// </remarks>
    [Post("/xpay/upload_vp_file")]
    Task<WxaXPayUploadVpFileResponse> UploadVpFileAsync(
        [Body] WxaXPayUploadVpFileRequest request,
        [Query("pay_sig")] string paySig,
        CancellationToken cancellationToken = default);
}
