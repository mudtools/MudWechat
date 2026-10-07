// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.OfficialAccount;

/// <summary>
/// 微信公众号 / 服务号「扫二维码打开小程序」域 SDK（4 端点，<b>服务号专属</b>）。
/// </summary>
/// <remarks>
/// <para>
/// <b>官方文档</b>：服务端 API 索引 <see href="https://developers.weixin.qq.com/doc/service/api/"/>
/// → 服务号二维码 → 扫二维码打开小程序。深链均为**逐页核验过的真实页面**（2026-10-07）。
/// </para>
/// <para>
/// <b>与带参二维码域的分工（勿混淆）</b>：本域管理<b>小程序跳转规则</b>（前缀规则 → 小程序页面），
/// 与 <see cref="IMpQrcodeService"/>（带参二维码 ticket 生成）是两族能力——
/// 官方索引虽同处「服务号二维码」分组，但官方「客服消息」分组同样被本仓库拆为
/// <c>KfAccount</c>/<c>KfSession</c>/<c>CustomerMessage</c> 三域 ⇒ 域边界按<b>功能族</b>而非官方分组。
/// </para>
/// <para>
/// <b>域级业务约束（逐页核验，勿弱化）</b>：
/// </para>
/// <list type="bullet">
/// <item><b>适用范围</b>：4 页均为「<b>服务号（仅认证）</b>」（SDK 不做账号类型本地闸，由官方错误码表达）。</item>
/// <item><b>频率限制</b>：官方错误码 <c>44990</c> 原文「接口请求太快，<b>超过 5 次/秒</b>」
/// ——4 页均无独立频次章节，5 次/秒为唯一数值（照录）。</item>
/// <item><b>发布配额</b>：<c>qrcodejumppublish</c> 的 <c>886000</c> 原文「<b>本月发布次数达到上限（100 次）</b>」；
/// <c>qrcodejumpget</c> 响应的 <c>qrcodejump_pub_quota</c> 即「本月还可发布的次数」（可用于前置探量）。</item>
/// <item><b>前置条件</b>：官方「服务号调用说明」原文要求<b>先关联小程序</b>（否则 <c>61007</c>，关联入口
/// <c>linkMiniprogram</c> 或公众号管理后台）；服务商代调用须先获授权限集 id 3。SDK <b>不编排</b>关联动作。</item>
/// <item><b>第三方平台</b>：4 页均支持代商家调用（<b>权限集 id 3、18</b>）——按 M0-R3 裁决只在 XML 记录，不扩实现面。</item>
/// <item><b>不做业务编排</b>：官方 <c>qrcodejumpadd</c> 页无「校验文件托管」正文（仅错误码 <c>85069</c>，
/// 属普通二维码场景）；SDK 只建模端点，托管与关联一律归宿主。</item>
/// </list>
/// <para>
/// <b>令牌路由</b>：全部端点消费 <see cref="MpTokenTypes.AccessToken"/> 并以 Query 注入
/// <c>access_token</c>（官方契约；MUD005 已知接受风险）。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "QrcodeJump", TokenManage = nameof(IMpAppManager))]
[Token(TokenType = MpTokenTypes.AccessToken,
       InjectionMode = TokenInjectionMode.Query,
       Name = "access_token")]
public interface IMpQrcodeJumpService
{
    /// <summary>
    /// 获取已设置的二维码规则。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/qrcode/qrcodejump/api_qrcodejumpget.html"/>
    /// （官方接口英文名 <c>qrcodeJumpGet</c>；官方标注<b>不支持云调用</b>）。
    /// </summary>
    /// <param name="request">查询请求（服务号场景需 <c>appid</c> 与 <c>get_type</c>；分页/前缀查询另带对应参数）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>规则列表（<c>rule_list</c>）与本月剩余发布配额（<c>qrcodejump_pub_quota</c>）等。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + 请求体见 <see cref="MpQrcodeJumpGetRequest"/>；
    /// 官方原文「若获取「扫普通二维码打开小程序」已设置的二维码规则，则<b>无需任何入参</b>」。
    /// </para>
    /// <para>官方错误码：<c>-1</c> / <c>0</c> / <c>40001</c> / <c>40097</c> / <c>40166</c> / <c>44990</c> / <c>85075</c> / <c>886001</c>。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/wxopen/qrcodejumpget")]
    Task<MpQrcodeJumpGetResponse> GetQrcodeJumpRulesAsync(
        [Body] MpQrcodeJumpGetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 增加或修改二维码规则。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/qrcode/qrcodejump/api_qrcodejumpadd.html"/>
    /// （官方接口英文名 <c>qrcodeJumpAdd</c>；官方标注<b>不支持云调用</b>）。
    /// </summary>
    /// <param name="request">规则请求（<c>prefix</c>/<c>path</c>/<c>is_edit</c> 必填；<c>appid</c> 与
    /// <c>open_version</c>/<c>debug_url</c>/<c>permit_sub_rule</c> 按场景携带）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + 请求体见 <see cref="MpQrcodeJumpAddRequest"/>；
    /// 官方原文「<b>已经发布的规则，不支持修改</b>」。
    /// </para>
    /// <para>
    /// 官方错误码（逐页核验，含常见链接类错误）：<c>-1</c> / <c>0</c> / <c>40001</c> / <c>40097</c>（已发布规则不支持修改）/
    /// <c>40166</c>（appid 不合法）/ <c>44990</c>（超 5 次/秒）/ <c>85066</c>（链接错误）/ <c>85068</c>（测试链接不是子链接）/
    /// <c>85069</c>（校验文件失败）/ <c>85070</c>（URL 命中黑名单）/ <c>85071</c>（链接重复）/ <c>85072</c>（链接被占用）/
    /// <c>85073</c>（规则数已满）/ <c>85075</c>（个人小程序限制）/ <c>85076</c>（check ICP fail）/ <c>886001</c>（系统繁忙）。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/wxopen/qrcodejumpadd")]
    Task<MpResponse> AddOrUpdateQrcodeJumpRuleAsync(
        [Body] MpQrcodeJumpAddRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 发布已设置的二维码规则。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/qrcode/qrcodejump/api_qrcodejumppublish.html"/>
    /// （官方接口英文名 <c>qrcodeJumpPublish</c>；官方标注<b>不支持云调用</b>）。
    /// </summary>
    /// <param name="request">发布请求（<c>prefix</c> 必填；服务号场景为服务号的带参二维码 url）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>
    /// <para>
    /// 官方契约：<b>POST</b> + 请求体 <c>{"prefix": …}</c>；须先经 <see cref="AddOrUpdateQrcodeJumpRuleAsync"/>
    /// 添加规则再发布；发布后扫码命中即跳转<b>正式版</b>小程序指定页面。
    /// </para>
    /// <para>
    /// <b>配额（官方错误码原文）</b>：<c>886000</c>「beyond publish count this month，本月发布次数达到上限（<b>100 次</b>）」；
    /// 其余错误码：<c>-1</c> / <c>0</c> / <c>40001</c> / <c>44990</c> / <c>45112</c> / <c>85074</c>（小程序未发布）/
    /// <c>85075</c> / <c>85095</c>（数据异常，请删除后重新添加）/ <c>886001</c>。
    /// </para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/wxopen/qrcodejumppublish")]
    Task<MpResponse> PublishQrcodeJumpRuleAsync(
        [Body] MpQrcodeJumpPublishRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除已设置的二维码规则。官方文档：<see href="https://developers.weixin.qq.com/doc/service/api/qrcode/qrcodejump/api_qrcodejumpdelete.html"/>
    /// （官方接口英文名 <c>qrcodeJumpDelete</c>；官方标注<b>不支持云调用</b>）。
    /// </summary>
    /// <param name="request">删除请求（<c>prefix</c> 必填；<c>appid</c> 为服务号场景需传）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>仅 <c>errcode</c>/<c>errmsg</c>（故返回 <see cref="MpResponse"/>）。</returns>
    /// <remarks>
    /// <para>官方契约：<b>POST</b> + 请求体见 <see cref="MpQrcodeJumpDeleteRequest"/>。</para>
    /// <para>官方错误码：<c>-1</c> / <c>0</c> / <c>44990</c> / <c>85075</c> / <c>886001</c>
    /// （本页错误码表未列 <c>40001</c>，照录）。</para>
    /// <para>MUD005：令牌强制走 Query 参数 <c>access_token</c>（官方契约，无法改用 Header）。</para>
    /// </remarks>
    [Post("/cgi-bin/wxopen/qrcodejumpdelete")]
    Task<MpResponse> DeleteQrcodeJumpRuleAsync(
        [Body] MpQrcodeJumpDeleteRequest request,
        CancellationToken cancellationToken = default);
}

/// <summary>二维码规则查询类型（官方 <c>get_type</c> 取值）。</summary>
public static class MpQrcodeJumpGetTypes
{
    /// <summary>查询最近新增的 10000 条（官方默认值 0）。</summary>
    public const int Recent = 0;

    /// <summary>按 <c>prefix_list</c> 查询（最多 200 个前缀）。</summary>
    public const int Prefix = 1;

    /// <summary>分页查询（按新增顺序返回）。</summary>
    public const int Paged = 2;
}

/// <summary>二维码规则发布标志位（官方 <c>state</c> / <c>rule_list[].state</c> 取值）。</summary>
public static class MpQrcodeJumpStates
{
    /// <summary>未发布。</summary>
    public const int Unpublished = 1;

    /// <summary>已发布。</summary>
    public const int Published = 2;
}

/// <summary>编辑标志位（官方 <c>is_edit</c> 取值）。</summary>
public static class MpQrcodeJumpEditFlags
{
    /// <summary>新增二维码规则。</summary>
    public const int Add = 0;

    /// <summary>修改已有二维码规则（<b>已经发布的规则不支持修改</b>）。</summary>
    public const int Edit = 1;
}

/// <summary>测试范围（官方 <c>open_version</c> 取值；仅「扫普通二维码」场景适用）。</summary>
public static class MpQrcodeJumpOpenVersions
{
    /// <summary>开发版（配置只对开发者生效）。</summary>
    public const int Develop = 1;

    /// <summary>体验版（配置对管理员、体验者生效）。</summary>
    public const int Trial = 2;

    /// <summary>正式版（配置对开发者、管理员和体验者生效）。</summary>
    public const int Release = 3;
}

/// <summary>
/// 子规则占用模式（官方 <c>permit_sub_rule</c> 取值；仅「扫普通二维码」场景适用）。
/// </summary>
/// <remarks>
/// 官方字段说明原文为「是否独占符合二维码前缀匹配规则的所有子规则」，取值「<c>1</c> 为不占用，<c>2</c> 为占用」
/// ——「是否独占」的方向与 1/2 语义的表述易误读，照录原文。
/// </remarks>
public static class MpQrcodeJumpSubRuleModes
{
    /// <summary>不占用（官方 1）。</summary>
    public const int NotOccupied = 1;

    /// <summary>占用（官方 2）。</summary>
    public const int Occupied = 2;
}
