// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.AccountId;

namespace Mud.Wechat.Work.Interfaces;

/// <summary>
/// 企业微信「账号ID」域「tmp_external_userid 转换」接口族公共 SDK。
/// <para>
/// 将应用获取的外部用户临时 id（tmp_external_userid，来自会议 / 收集表 / 智能表等业务接口）转换为
/// external_userid 或 userid。官方对<b>企业自建应用、第三方应用与服务商代开发开放完全一致</b>的
/// 1 个端点（98729/98412/98741），声明于本公共父接口；
/// 自建/第三方/代开发应用类型子接口均为空标记，仅作为类型化契约入口。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 令牌路由键为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>）；
/// 调用此接口的应用，和获取到 tmp_external_userid 的应用必须是同一个；
/// user_type 为 1（客户类型）时应用还需具有「客户联系」权限。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkAccountIdTmpExternalUserIdService
{
    /// <summary>
    /// tmp_external_userid 的转换
    /// <para>将应用获取的外部用户临时 id 转换为 external_userid（user_type=1 客户）或
    /// corpid + userid（user_type=2 企业互联 / 3 上下游 / 4 互联企业（圈子））。</para>
    /// <para>支持的业务类型（business_type）：1 - 会议，2 - 收集表，3 - 智能表（文档）。</para>
    /// </summary>
    /// <param name="request">转换请求体（<see cref="ConvertTmpExternalUserIdRequest"/>；最多不超过 100 个）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>转换成功结果列表（results，按 user_type 返回不同字段）及无法转换列表（invalid_tmp_external_userid_list）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98729"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98412"/></para>
    /// <para><b>服务商代开发</b>SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/98741"/></para>
    /// </remarks>
    [Post("/cgi-bin/idconvert/convert_tmp_external_userid")]
    Task<ConvertTmpExternalUserIdResponse> ConvertTmpExternalUserIdAsync(
        [Body] ConvertTmpExternalUserIdRequest request,
        CancellationToken cancellationToken = default);
}
