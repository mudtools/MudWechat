// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Dial;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「公费电话」模块企业自建应用 SDK（获取公费电话拨打记录）。
/// <para>
/// 官方仅向自建应用开放本域 1 个端点（代开发应用与第三方应用均暂不支持），
/// 声明于本接口（形态对齐 <see cref="IWechatWorkInternalMsgAuditPermitUserService"/>）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>）。
/// 官方权限口径：调用应用需配置到「公费电话 - 可调用接口的应用」中。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Dial",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkDialService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalDialService : IWechatWorkDialService
{
    /// <summary>
    /// 获取公费电话拨打记录
    /// <para>按时间范围分页查询公费电话拨打记录（主被叫、通话类型与时长）。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetDialRecordRequest"/>：start_time / end_time / offset / limit）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>拨打记录列表（record：call_time / total_duration / call_type / caller / callee）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/93662"/></para>
    /// <para>官方业务限制：代开发应用、第三方应用均暂不支持本接口；
    /// 查询范围为 [start_time, end_time] 双闭区间，结束时间不得小于开始时间，否则返回 600018（无效的起止时间）；
    /// 起止时间最大跨度 30 天，超过则以结束时间为基准向前取 30 天；
    /// 未指定起止时间时默认查询最近 30 天范围内数据；
    /// 应用可见范围外用户相关的记录会被过滤。</para>
    /// </remarks>
    [Post("/cgi-bin/dial/get_dial_record")]
    Task<GetDialRecordResponse> GetDialRecordAsync(
        [Body] GetDialRecordRequest request,
        CancellationToken cancellationToken = default);
}
