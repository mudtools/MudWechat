// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Checkin;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「打卡」模块设备打卡数据域公共 SDK（获取设备打卡数据；路由挂 /cgi-bin/hardware/ 域）。
/// <para>
/// 官方对三类应用开放一致的「获取设备打卡数据」端点，收敛声明于本接口；应用类型子接口均为零差异端点空标记：
/// 自建应用见 <see cref="IWechatWorkInternalCheckinDeviceService"/>，
/// 服务商代开发见 <see cref="IWechatWorkProviderCheckinDeviceService"/>，
/// 第三方应用见 <see cref="IWechatWorkThirdPartyCheckinDeviceService"/>。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 令牌路由键说明：三类应用消费的令牌路由键均为 <see cref="WechatTokenTypes.AccessToken"/>（Query 注入 <c>access_token</c>）——
/// 自建应用消费应用自身 access_token；第三方/服务商代开发消费授权企业级 access_token（scope = authCorpId），
/// 须先经 <c>IWechatAppContextSwitcher</c> 切换作用域后再调用。官方权限口径：自建应用须配置到
/// 「打卡 - 可调用接口的应用」中；代开发应用须具有「打卡」权限；第三方应用须具有「打卡」权限且设备型号需关联该第三方应用。
/// 自 2023 年 12 月 1 日 0 点起，不再支持通过系统应用 secret 调用接口（存量企业暂不受影响）。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(TokenManage = nameof(IWechatAppManager), IsAbstract = true)]
[Token(TokenType = WechatTokenTypes.AccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkCheckinDeviceService
{
    /// <summary>
    /// 获取设备打卡数据
    /// <para>获取可见范围内成员在考勤设备上产生的原始打卡记录，包括未被打卡应用记录的不符合打卡规则的记录。</para>
    /// <para>官方限制：获取记录时间跨度不超过一个月；用户列表不超过 100 个，若超过请分批获取；获取的是通过考勤设备打卡的原始记录，不包含企业微信 app 手机打卡的记录；userid 无效时忽略该参数，不报错。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="GetHardwareCheckinDataRequest"/>：filter_type 过滤类型 / starttime / endtime / useridlist 用户列表）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>设备打卡记录列表（checkindata：userid / checkin_time / device_sn / device_name）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/94126"/></para>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/95176"/></para>
    /// <para><b>服务商代开发</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96504"/></para>
    /// <para>官方权限：自建应用须配置到「打卡 - 可调用接口的应用」中；代开发应用须具有「打卡」权限；第三方应用须具有「打卡」权限且设备型号需关联该第三方应用。</para>
    /// <para>官方契约陷阱：路由挂 /cgi-bin/hardware/ 域（get_hardware_checkin_data）而非 /cgi-bin/checkin/ 域；filter_type 决定 starttime/endtime 的语义（打卡时间 vs 设备上传时间，默认 1）。</para>
    /// </remarks>
    [Post("/cgi-bin/hardware/get_hardware_checkin_data")]
    Task<GetHardwareCheckinDataResponse> GetHardwareCheckinDataAsync(
        [Body] GetHardwareCheckinDataRequest request,
        CancellationToken cancellationToken = default);
}
