// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「品牌申请」域 SDK（2 端点：品牌申请 / 小程序品牌申请状态查询；
/// 官方 order_shipping 分区 <c>/wxa/sec/famousbrand/*</c> 路由族）。
/// </summary>
/// <remarks>
/// <para>
/// <b>路由族与订单发货域不同</b>：本域是 <c>/wxa/sec/famousbrand/</c>（2 条），订单面是
/// <c>/wxa/sec/order/</c>（10 条）⇒ 官方把两支放在同一 order_shipping 目录树下，本仓按两个路由族
/// 分接口、共用一个注册组（守卫 OS1 锁定两族计数）。
/// </para>
/// <para>
/// <b>令牌形态（MP-X2 / MP-X3 同口径）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入
/// <c>access_token</c>；两支均支持第三方平台以 <c>authorizer_access_token</c> 代调用（权限集 142），
/// 两支均<b>不支持云调用</b>。
/// </para>
/// <para>
/// <b>适用范围段与同域其它端点不同</b>：本域两页正文写的是「本接口支持「小程序」账号类型调用。
/// 其他账号类型如无特殊说明，均不可调用」，而 <c>/wxa/sec/order/*</c> 十页均为「暂未明确可调用账号类型」
/// ⇒ 这是本线少数带<b>正面账号类型限定</b>的域，宿主不要把「其它线也能调」当成默认。
/// </para>
/// <para><b>调用限制</b>：两端点官方正文均未公布端点级频率数字。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "OrderShipping", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaFamousBrandService
{
    /// <summary>
    /// 品牌申请（<c>POST /wxa/sec/famousbrand/apply</c>，<b>不支持云调用</b>，权限集 142）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_famousbrandapply.html"/>。
    /// </summary>
    /// <param name="request">请求体（包装键为首字母大写的 <c>Application</c>，其内 <c>apply_for</c> 必填），见 <see cref="WxaFamousBrandApplyRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>（官方返回体表除通用两字段外无业务字段）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + JSON 请求体，注意事项原文点名「Content-Type 需要指定为 application/json」。</para>
    /// <para>
    /// <b><c>Application</c> 是全仓唯一首字母大写的请求体包装键</b>（官方请求体表与示例逐字如此）
    /// ⇒ <c>JsonPropertyName</c> 不得改写成小写形态，守卫 OS4 锁定该拼写。
    /// </para>
    /// <para>
    /// <b>材料条件必填</b>：<c>audit_info</c> 在申请类型为「知名品牌（<c>1</c>）」时必填；品牌类型 <c>4</c>
    /// （电商平台旗舰店）要求 <c>flagship_in_which_ec_platform</c> + <c>ec_platform_proof_list</c>，
    /// 品牌类型 <c>7</c>（驰名商标、著名商标）要求 <c>authority_certified_proof_list</c>。
    /// 申请类型 <c>2</c>（已接入小店商品组件）不需要品牌材料。
    /// </para>
    /// <para><b>三个佐证数组的元素是 <c>media_id</c> 字符串</b>（官方示例为临时素材 id 形态）：须先走「新增临时素材」上传图片，临时素材有效期约束下不得长期复用。</para>
    /// <para><b>错误码 <c>10210003</c>「图片解析失败」</b>说明材料校验发生在服务端解析阶段 ⇒ 提交成功不等于材料可用，须回读 <see cref="GetApplyStatusAsync"/> 的 <c>audit_reason</c>。</para>
    /// </remarks>
    [Post("/wxa/sec/famousbrand/apply")]
    Task<WxaResponse> ApplyAsync(
        [Body] WxaFamousBrandApplyRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 小程序品牌申请状态查询（<c>POST /wxa/sec/famousbrand/get_status</c>，<b>不支持云调用</b>，权限集 142）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/order_shipping/api_getfamousbrandapplystatus.html"/>。
    /// </summary>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>申请进度（<c>progress.status</c>）与申请信息（<c>application</c>，含驳回原因），见 <see cref="WxaFamousBrandApplyStatusResponse"/>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b>，请求体表原文为「无」⇒ 本端点无 <c>[Body]</c> 参数，由守卫 OS3 与 <c>/xpay</c> 域同口径锁定零请求体面。</para>
    /// <para>
    /// <b>页面自相矛盾处（留档不改契约）</b>：请求体表写「无」，而同页注意事项要求 <c>Content-Type: application/json</c>、
    /// 错误码表含 <c>44002</c>「POST内容为空」⇒ 本仓按<b>请求体表</b>（权威参数面）落地为无请求体；
    /// 若宿主实测得到 <c>44002</c>，属官方页面口径待证，<b>不得</b>自行加空对象参数来绕过。
    /// </para>
    /// <para><b>状态在两处给出</b>：<c>progress.status</c> 与 <c>application.status</c> 共用一张取值表（<see cref="WxaFamousBrandApplyStatusValues"/>，1 审核中 / 2 驳回 / 3 通过）。</para>
    /// <para><b>官方返回示例两处均为 <c>0</c>，而枚举表只有 1/2/3</b> ⇒ 本表不收录 0，也不得把 0 解释成某种业务态（官方正文无此口径）。</para>
    /// <para><b>驳回原因只在驳回态出现</b>（官方原文「只有在审核驳回时才有值」）⇒ 通过态下 <c>audit_reason</c> 为空属正常，不得当作数据缺失告警。</para>
    /// <para><b>本域错误码含令牌与 IP 白名单族</b>（<c>41001</c> / <c>42001</c> / <c>61004</c>）：这两支在 <c>/wxa/sec/order/*</c> 页面未出现，属页面码表差异。</para>
    /// </remarks>
    [Post("/wxa/sec/famousbrand/get_status")]
    Task<WxaFamousBrandApplyStatusResponse> GetApplyStatusAsync(
        CancellationToken cancellationToken = default);
}
