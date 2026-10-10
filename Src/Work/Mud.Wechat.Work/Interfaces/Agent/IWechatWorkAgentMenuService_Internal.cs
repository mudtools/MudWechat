// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.Agent;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「应用管理」模块「自定义菜单」域企业自建应用 SDK
/// （创建菜单 + 获取菜单 + 删除菜单，全部 3 端点官方仅自建应用开放）。
/// <para>
/// 自定义菜单端点的官方权限说明均为「仅企业可调用；第三方不可调用」，
/// 服务商代开发章节亦无对应 API，故不设第三方/代开发子接口。
/// 三个端点的 agentid 均为 Query 参数（官方契约），菜单结构经请求体/响应体承载。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 消费应用自身 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，Query 注入 <c>access_token</c>）。
/// 官方权限口径：仅企业可调用；第三方不可调用。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Agent",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkAgentMenuService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.InternalAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkInternalAgentMenuService : IWechatWorkAgentMenuService
{
    /// <summary>
    /// 创建菜单
    /// <para>为指定应用创建自定义菜单（一级菜单数组 + 二级菜单数组）。</para>
    /// <para>官方限制：一级菜单（button）个数为 1~3 个；二级菜单（sub_button）个数为 1~5 个；
    /// 主菜单名不超过 16 字节、子菜单名不超过 40 字节；菜单 KEY 值不超过 128 字节；
    /// 网页链接不超过 1024 字节（建议使用 https）；appid 仅限与企业绑定的小程序。
    /// 响应动作类型（type）支持：click（点击推事件）/ view（跳转 URL）/ scancode_push（扫码推事件）/
    /// scancode_waitmsg（扫码推事件且弹出提示）/ pic_sysphoto（弹出系统拍照发图）/
    /// pic_photo_or_album（弹出拍照或者相册发图）/ pic_weixin（弹出企业微信相册发图器）/
    /// location_select（弹出地理位置选择器）/ view_miniprogram（跳转到小程序）。</para>
    /// </summary>
    /// <param name="agentid">企业应用 id（官方必填；可在应用管理设置页查看）。</param>
    /// <param name="request">请求体（<see cref="CreateAgentMenuRequest"/>：button——菜单按钮数组，详见 <see cref="AgentMenuButton"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90231"/></para>
    /// <para>官方权限：仅企业可调用；第三方不可调用。</para>
    /// </remarks>
    [Post("/cgi-bin/menu/create")]
    Task<WechatWorkResponse> CreateMenuAsync(
        [Query("agentid")] int agentid,
        [Body] CreateAgentMenuRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取菜单
    /// <para>获取指定应用的菜单结构，返回结果与创建菜单时的 button 数据结构一致。</para>
    /// <para>官方限制：仅企业可调用；第三方不可调用。</para>
    /// </summary>
    /// <param name="agentid">企业应用 id（官方必填；可在应用管理设置页查看）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>菜单结构（button：菜单按钮数组，含二级菜单 sub_button）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90232"/></para>
    /// <para>官方权限：仅企业可调用；第三方不可调用。</para>
    /// </remarks>
    [Get("/cgi-bin/menu/get")]
    Task<GetAgentMenuResponse> GetMenuAsync(
        [Query("agentid")] int agentid,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除菜单
    /// <para>删除指定应用的全部自定义菜单（覆盖式删除，无法按单个菜单项删除）。</para>
    /// <para>官方限制：仅企业可调用；第三方不可调用。</para>
    /// </summary>
    /// <param name="agentid">企业应用 id（官方必填；可在应用管理设置页查看）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅返回 errcode / errmsg（无业务负载）。</returns>
    /// <remarks>
    /// <para><b>企业自建应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/90233"/></para>
    /// <para>官方权限：仅企业可调用；第三方不可调用。</para>
    /// </remarks>
    [Get("/cgi-bin/menu/delete")]
    Task<WechatWorkResponse> DeleteMenuAsync(
        [Query("agentid")] int agentid,
        CancellationToken cancellationToken = default);
}
