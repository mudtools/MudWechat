// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Pay.Abstractions.Configuration;

namespace Mud.Wechat.Pay.Abstractions.Credential;

/// <summary>
/// 多商户基座端口：按 <see cref="WechatPayMerchantConfig.MerchantKey"/> 分槽持有商户凭据配置。
/// </summary>
/// <remarks>
/// <para>
/// 支付线天然多商户（一个 SDK 宿主可能代多个商户收款），故从第一天就按「多商户」建模，
/// 而非先做单商户再改签名 —— 后者会让全部接口签名变成「取默认商户」的隐式依赖。
/// </para>
/// <para>
/// 落位在 <b>Abstractions</b>：回调包按通知中的 <c>mchid</c> 反查商户时同样要读本端口，
/// 而 <c>Callback → Pay</c> 违反依赖单向（与 P0-b/P0-c 同一理由）。
/// </para>
/// <para>
/// <b>只读配置面</b>：本端口不管理令牌、不缓存密钥、不做证书刷新 —— 那些属 P1 阶段的
/// 签名通道与平台证书刷新，避免在此提前引入无消费点的机制。
/// </para>
/// </remarks>
public interface IWechatPayMerchantManager
{
    /// <summary>全部已注册商户（按注册顺序）。</summary>
    IReadOnlyList<WechatPayMerchantConfig> Merchants { get; }

    /// <summary>
    /// 按商户键查询；查不到返回 <c>false</c>（回调侧对未知 <c>mchid</c> 用此分支告警而非抛异常）。
    /// </summary>
    /// <param name="merchantKey">商户键（普通商户为 <c>mchid</c>，服务商为 <c>{sp_mchid}:{sub_mchid}</c>）。</param>
    /// <param name="config">命中的商户配置。</param>
    /// <returns>是否命中。</returns>
    bool TryGetMerchant(string merchantKey, out WechatPayMerchantConfig? config);

    /// <summary>
    /// 按商户键查询，查不到即抛（发起请求前的显式调用，未知商户属编程错误而非可恢复状态）。
    /// </summary>
    /// <param name="merchantKey">商户键。</param>
    /// <returns>命中的商户配置。</returns>
    /// <exception cref="InvalidOperationException">商户键未注册时抛出。</exception>
    WechatPayMerchantConfig GetMerchant(string merchantKey);

    /// <summary>
    /// 按<b>官方报文中的商户标识</b>反查商户（回调通知接收方、账单归属方的定位入口）。
    /// </summary>
    /// <remarks>
    /// 官方回调载荷里，普通商户只带 <c>mchid</c>；服务商带 <c>sp_mchid</c> + <c>sub_mchid</c>
    /// （且此时的 <c>mchid</c> 字段即服务商商户号）。本方法把该分流与
    /// <see cref="WechatPayMerchantConfig.MerchantKey"/> 的复合规则收敛在<b>一处</b>，
    /// 避免回调包各自拼接键而与配置侧漂移。
    /// </remarks>
    /// <param name="mchId">报文中的 <c>mchid</c>（普通商户为商户号；服务商形态可传 <c>null</c>）。</param>
    /// <param name="spMchId">报文中的 <c>sp_mchid</c>（普通商户形态传 <c>null</c>）。</param>
    /// <param name="subMchId">报文中的 <c>sub_mchid</c>（普通商户形态传 <c>null</c>）。</param>
    /// <param name="config">命中的商户配置。</param>
    /// <returns>是否命中（未命中时调用方应告警并按畸形/未知商户处理，不得回落到默认商户）。</returns>
    bool TryGetByOfficialIds(
        string? mchId, string? spMchId, string? subMchId,
        out WechatPayMerchantConfig? config);
}
