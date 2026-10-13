// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.MiniProgram;

/// <summary>
/// 微信小程序「直播 · 成员角色」域 SDK（3 端点：设置角色 / 移除角色 / 查询成员列表；
/// 官方 livebroadcast 分区 → role-management）。
/// </summary>
/// <remarks>
/// <para>
/// <b>账号级权限面</b>：角色是「小程序成员」维度的身份（超级管理员 / 管理员 / 主播 / 运营者），
/// 与房间维度的小助手、主播副号（<see cref="IWxaLiveAssistantService"/>）分属两套官方目录，
/// 不可互相替代。设置主播角色时官方会走实名认证校验（错误码 <c>400002</c>）。
/// </para>
/// <para>
/// <b>角色可多选</b>：查询结果的 <c>roleList</c> 是数组（一个成员可同时是主播与运营者），
/// 而写端点入参 <c>role</c> 是单值 ⇒ 「一次设一职」，多职须多次调用；上限官方以 <c>400003</c>
/// 表述（管理员 10 个、运营者 500 个、主播 500 个）。
/// </para>
/// <para>
/// <b>令牌形态（MP-X2 / MP-X3 同口径）</b>：复用 <c>MpTokenTypes.AccessToken</c> + Query 注入
/// <c>access_token</c>（权限集 52）。域级取证、「适用范围字段异常」留档与 GET 参数落地裁决见
/// <see cref="IWxaLiveRoomService"/> 注释。
/// </para>
/// <para><b>调用限制</b>：三支端点官方均标注「本接口无特殊注意事项」（未公布额度数字）。</para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "Live", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IWxaLiveRoleService
{
    /// <summary>
    /// 设置成员角色（<c>POST /wxaapi/broadcast/role/addrole</c>，云调用 <c>liveBroadcast.addRole</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/role-management/api_getrolelistdw.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>username</c> + <c>role</c>；与 <see cref="DeleteRoleAsync"/> 共用 <see cref="WxaLiveRoleRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns><c>codeurl</c>（实名验证小程序码链接），见 <see cref="WxaLiveAddRoleResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b>；<b>页面名与路由严重交叉（照录官方）</b>：本页文件名为
    /// <c>api_getrolelistdw</c>、接口英文名 <c>getRoleListdw</c>，正文标题与路由却是「设置成员角色」
    /// <c>role/addrole</c>（云调用 <c>liveBroadcast.addRole</c>）⇒ URL 取官方文件名所在页，
    /// 方法名按路由与标题取 <see cref="AddRoleAsync"/>；查询成员列表另有 <c>api_getrolelist</c> 页，
    /// 见 <see cref="GetRoleListAsync"/>。
    /// </para>
    /// <para>官方错误码：<c>400001</c>（微信号不合规）/ <c>400002</c>（微信号需实名，仅设主播时）/ <c>400003</c>（达上限）/ <c>400004</c>（重复添加）。</para>
    /// <para><b>成功示例不含 <c>codeurl</c></b>（官方示例只回 <c>{errcode, errmsg}</c>）⇒ 该字段仅在需引导实名时出现，宿主侧按空值分支处理。</para>
    /// </remarks>
    [Post("/wxaapi/broadcast/role/addrole")]
    Task<WxaLiveAddRoleResponse> AddRoleAsync(
        [Body] WxaLiveRoleRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 移除成员角色（<c>POST /wxaapi/broadcast/role/deleterole</c>，云调用 <c>liveBroadcast.deleteRole</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/role-management/api_deleterole.html"/>。
    /// </summary>
    /// <param name="request">请求体（<c>username</c> + <c>role</c>，共用 <see cref="WxaLiveRoleRequest"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c> / <c>errmsg</c>。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b>；本接口官方标注「无特殊注意事项」。</para>
    /// <para>
    /// 官方错误码：<c>2003</c>（官方表中描述与解决方案均为空，照录不做推测）/ <c>400001</c> /
    /// <c>400005</c>（主播角色删除失败，该主播存在未开播的直播间）。
    /// </para>
    /// <para><b>与 <c>addrole</c> 共用请求体</b>：官方两页参数表逐字段相同（<c>username</c> + <c>role</c>）⇒ DTO 分立反而误导，守卫 LB4 锁定该共用关系。</para>
    /// </remarks>
    [Post("/wxaapi/broadcast/role/deleterole")]
    Task<WxaResponse> DeleteRoleAsync(
        [Body] WxaLiveRoleRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 查询成员列表（<c>GET /wxaapi/broadcast/role/getrolelist</c>，云调用 <c>liveBroadcast.getRoleList</c>）。
    /// 官方文档：<see href="https://developers.weixin.qq.com/miniprogram/dev/server/API/livebroadcast/role-management/api_getrolelist.html"/>。
    /// </summary>
    /// <param name="offset">起始偏移量（<c>offset</c>，选填，默认 0）。</param>
    /// <param name="limit">查询个数（<c>limit</c>，选填，默认 10，最大 30）。</param>
    /// <param name="keyword">搜索词（<c>keyword</c>，选填：微信号或昵称，不传则返回全部）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>总个数 + 成员列表，见 <see cref="WxaLiveRoleListResponse"/>。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>GET</b>，三个参数均写在请求地址行（<c>?access_token=…&amp;offset=…&amp;limit=…&amp;keyword=…</c>）
    /// 且官方全部标注选填；本接口官方标注「无特殊注意事项」。官方错误码：<c>40001</c>。
    /// </para>
    /// <para>
    /// <b>响应字段名自成一套</b>：<c>headingimg</c>（头像）/ <c>roleList</c>（numarray，复数身份）/
    /// <c>updateTimestamp</c>（官方类型列为 string）⇒ 照录原文，见 <see cref="WxaLiveRoleItem"/>。
    /// </para>
    /// <para><b><c>username</c> 官方已脱敏</b>（示例值 <c>"o0****0o"</c>）⇒ 不得把该字段用作回写键，回写须用 <c>openid</c> 对应的原始微信号（宿主自持）。</para>
    /// </remarks>
    [Get("/wxaapi/broadcast/role/getrolelist")]
    Task<WxaLiveRoleListResponse> GetRoleListAsync(
        [Query("offset")] long? offset = null,
        [Query("limit")] long? limit = null,
        [Query("keyword")] string? keyword = null,
        CancellationToken cancellationToken = default);
}
