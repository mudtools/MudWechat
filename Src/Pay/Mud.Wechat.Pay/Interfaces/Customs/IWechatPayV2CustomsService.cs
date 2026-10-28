// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Pay.V2;

/// <summary>
/// 微信支付 APIv2 <b>清关报关（订单附加信息）</b>域 3 支端点（申报 / 查询 / 重推）。
/// </summary>
/// <remarks>
/// <para><b>官方文档</b>（APIv2 商户侧，2026-10-18 逐页核验）：
/// 订单附加信息提交 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011985151"/>、
/// 订单附加信息查询 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011985273"/>、
/// 订单附加信息重推 <see href="https://pay.weixin.qq.com/doc/v2/merchant/4011985318"/>。</para>
/// <para><b>本族是 APIv2 里的「无 nonce_str 例外」</b>：三页的请求与应答面<b>都没有</b>
/// <c>nonce_str</c>（随机字符串），申报页的请求侧还<b>没有</b> <c>sign_type</c>
/// （但应答里有且标注「暂只支持 MD5」）。传输层按字段表签名 ⇒ 空值不入库，
/// 请勿为「凑齐通用形状」而给请求补这两个字段。</para>
/// <para><b>全部 3 支不需证书</b>（三页「是否需要证书」栏均为「否」）⇒ 整族声明式生成，
/// 域名 <c>api.mch.weixin.qq.com</c>（三页均未出现 <c>api2.mch</c>）。</para>
/// <para><b>前置开通</b>：官方在申报与重推两页的「重点注意」要求先在
/// <c>http://pay.weixin.qq.com/index.php/extend/customs</c> 提交商户的海关信息；
/// 未开通自助清关功能时报 <c>AUTHORITY_NOT_FOUND</c>，未备案该海关时报 <c>CUSTOMSCONFIG_NOT_SET</c>。</para>
/// <para><b>拆单判定只看一个字段</b>：官方原文「商户传了 <c>sub_order_no</c> 就认为是拆单」⇒
/// 传了子订单号就必须同时传 <c>fee_type</c>/<c>order_fee</c>/<c>transport_fee</c>/<c>product_fee</c>，
/// 且满足 <c>order_fee = transport_fee + product_fee</c>，否则 <c>132011004</c>。</para>
/// <para><b>个人信息</b>：申报请求的 <c>cert_id</c>/<c>name</c> 是订购人实名信息，
/// SDK 不落日志、不入异常消息（PAY-B7 同线口径）。</para>
/// </remarks>
[AllowAnyStatusCode]
[HttpClientApi(RegistryGroupName = "CustomsV2", HttpClient = WechatPayV2HttpClientNames.TypeName)]
public interface IWechatPayV2CustomsService
{
    /// <summary>
    /// 报关信息申报（订单附加信息提交）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/merchant/4011985151"/>。
    /// </summary>
    /// <param name="request">申报请求体，见 <see cref="V2CustomsDeclareRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>申报结果（<c>state</c> + 验核机构信息），见 <see cref="V2CustomsDeclareResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/cgi-bin/mch/customs/customdeclareorder</c>；
    /// XML 报文；签名方式<b>只有 MD5</b>。</para>
    /// <para><b>金额以支付系统为准</b>：未拆单时商户<b>无需上传</b>订单金额（官方原文），
    /// 上传了反而与系统记录冲突；部分退款后申报须按退款后的余额口径。</para>
    /// <para><b>身份信息只作校验</b>：<c>cert_id</c>/<c>name</c> 上传后若与微信侧不一致，
    /// 官方回 <c>cert_check_result = DIFFERENT</c> 但<b>不影响报关结果</b>；
    /// 未实名用户须先完成实名再报关。</para>
    /// <para><b>验核机构映射</b>：应答的 <c>verify_department</c> 需由商户映射成海关的
    /// <c>verDept</c>（UNIONPAY→1 / NETSUNION→2 / OTHERS→3）后再报备海关。</para>
    /// </remarks>
    [Post("/cgi-bin/mch/customs/customdeclareorder")]
    Task<V2CustomsDeclareResponse> DeclareAsync(
        V2CustomsDeclareRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 报关信息查询（订单附加信息查询）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/merchant/4011985273"/>。
    /// </summary>
    /// <param name="request">查询请求体，见 <see cref="V2CustomsQueryRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>顶层固定字段 + <c>count</c> 条下标记录，见 <see cref="V2CustomsQueryResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/cgi-bin/mch/customs/customdeclarequery</c>；XML；MD5。</para>
    /// <para><b>四个订单标识四选一</b>，官方优先级 <c>sub_order_id</c> &gt; <c>sub_order_no</c>
    /// &gt; <c>transaction_id</c> &gt; <c>out_trade_no</c>；<c>customs</c> 恒必填。</para>
    /// <para><b>应答是下标平铺键</b>：记录字段以 <c>state_0</c>/<c>order_fee_0</c>/<c>customs_1</c> … 形态
    /// 出现在 XML 根下（<c>$n</c> 取 0 ~ <c>count</c>-1），只有 <c>verify_department</c> 与
    /// <c>verify_department_trade_id</c> 不带下标 ⇒ 落在 <see cref="V2CustomsQueryResponse.IndexedFields"/>。</para>
    /// <para><b>枚举值与请求侧不同</b>：应答的 <c>customs_$n</c> 是<b>无 <c>_ZS</c> 后缀</b>的另一套
    /// （GUANGZHOU / HANGZHOU / NINGBO / ZHENGZHOU_BS / CHONGQING / SHANGHAI / ZHENGZHOU_ZH），
    /// 且官方标注 String(2) —— 长度与实际取值矛盾，照抄官方、不得「顺手修正」。</para>
    /// <para><b>本页无错误码表</b>（其 err_code 说明指向不存在的「第 6 节」）。</para>
    /// </remarks>
    [Post("/cgi-bin/mch/customs/customdeclarequery")]
    Task<V2CustomsQueryResponse> QueryAsync(
        V2CustomsQueryRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// 报关信息重推（订单附加信息重推）。
    /// 官方文档：<see href="https://pay.weixin.qq.com/doc/v2/merchant/4011985318"/>。
    /// </summary>
    /// <param name="request">重推请求体，见 <see cref="V2CustomsRedeclareRequest"/>。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/> 取消操作令牌对象。</param>
    /// <returns>重推结果（扁平单记录，无验核机构字段），见 <see cref="V2CustomsRedeclareResponse"/>。</returns>
    /// <remarks>
    /// <para><b>官方契约</b>：<b>POST</b> <c>/cgi-bin/mch/<b>newcustoms</b>/customdeclareredeclare</c>
    /// —— 路径段与前两支的 <c>mch/customs</c> 不同，属官方原文，勿统一。</para>
    /// <para><b>用途</b>：申报失败（<c>state</c> 为 <c>FAIL</c> / <c>EXCEPT</c>）后按海关要求重新推送支付单。</para>
    /// <para><b>定位方式</b>：<c>out_trade_no</c> 与 <c>transaction_id</c> 二选一；
    /// 涉及拆单时 <c>sub_order_no</c> 与 <c>sub_order_id</c> 至少传一个。</para>
    /// <para><b>应答面比申报页窄</b>：没有 <c>cert_check_result</c> 与 <c>verify_department*</c>，
    /// 只有 <c>state</c> / <c>explanation</c> / <c>modify_time</c> 等 ⇒ 重推后如需验核机构流水号，须另调查询。</para>
    /// </remarks>
    [Post("/cgi-bin/mch/newcustoms/customdeclareredeclare")]
    Task<V2CustomsRedeclareResponse> RedeclareAsync(
        V2CustomsRedeclareRequest request, CancellationToken cancellationToken = default);
}
