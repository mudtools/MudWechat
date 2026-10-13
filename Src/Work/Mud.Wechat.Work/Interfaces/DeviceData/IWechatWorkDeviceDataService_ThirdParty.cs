// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

using Mud.Wechat.Work.DataModels.DeviceData;
using Mud.Wechat.Work.Interfaces;

namespace Mud.Wechat.Work;

/// <summary>
/// 企业微信「智慧硬件 / 智能设备数据」模块第三方应用 SDK
/// （获取授权设备信息 + 打卡 / 温度检测 / 门禁通行三条数据面 + 门禁规则增删改查，共 8 端点）。
/// <para>
/// 官方「智慧硬件服务端」文档树只在第三方应用开发根下存在页面，本域 8 个端点全部声明于本接口
/// （形态对齐 <see cref="IWechatWorkInternalMeetingRoomService"/> 与 <see cref="IWechatWorkInternalKfKnowledgeService"/>）。
/// </para>
/// </summary>
/// <remarks>
/// <para>
/// 消费授权企业级 access_token（路由键 <see cref="WechatTokenTypes.AccessToken"/>，企业级令牌一企一份，
/// 宿主须以授权企业 scope 获取后再调用）。
/// </para>
/// <para>
/// 与「智能硬件开发」根树下的 <c>/cgi-bin/openhw/*</c> 分属官方两棵章节树（openhw 侧凭据链为
/// <c>model_token</c> / <c>device_token</c> 与设备签名，不属三类应用令牌面）。
/// </para>
/// <para>
/// 三条数据面共用参数语义（打卡 / 温度检测 / 门禁通行三页正文口径一致）：<c>user_type</c> 为 0-企业成员 / 2-学生；
/// <c>data_filter_type</c> 为 1-按设备 sn 筛选（此时 <c>device_sn_list</c> 必填、忽略 <c>open_userid_list</c>）、
/// 2-按成员 openuserid 筛选（此时 <c>open_userid_list</c> 必填、忽略 <c>device_sn_list</c>）；
/// <c>agentid</c> 在三页参数表均标为「否」，但描述为「第三方套件应用必填，第三方单应用可不填」。
/// </para>
/// <para>
/// 本域 8 个页面均未给出独立的数值型频率限制，走官方「附录 - 访问频率限制」的默认阈值
/// （每企业调用单个 cgi/api 不可超过 1 万次/分、15 万次/小时；第三方应用提供商每 IP 不可超过 4 万次/分、120 万次/小时），
/// <see href="https://developer.work.weixin.qq.com/document/path/90312"/>。
/// 各端点<b>不</b>写「接口调用频率限制为 N 次/分钟」——该空缺是官方正文口径，不得凭第三方 SDK 注释或记忆补数。
/// </para>
/// <para>
/// MUD005 已知接受风险：企业微信官方契约强制令牌走 Query 参数（<c>access_token</c>），无法改用 Header。
/// </para>
/// </remarks>
[HttpClientApi(RegistryGroupName = "DeviceData",
    TokenManage = nameof(IWechatAppManager), InheritedFrom = nameof(WechatWorkDeviceDataService))]
[Token(TokenType = WechatTokenTypes.AccessToken,
      TokenManagerKey = WechatTokenManagerKeys.CorpAccessToken,
      InjectionMode = TokenInjectionMode.Query, Name = "access_token")]
public interface IWechatWorkThirdPartyDeviceDataService : IWechatWorkDeviceDataService
{
    /// <summary>
    /// 获取硬件授权结果
    /// <para>查询当前授权企业在本应用下可访问的智能设备列表（设备类型、序列号与型号 / 出厂名 / 备注名）。</para>
    /// <para>官方限制：本页面未给出独立的数值型频率限制，走官方全局访问频率限制（默认阈值见本接口 <c>remarks</c>）；
    /// <c>agentid</c> 在官方参数表标注为「否」（可选），描述为「应用id，仅套件应用需要填写该字段」。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="DeviceDataAuthInfoRequest"/>：agentid 官方参数表标注为可选）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>授权设备列表（<see cref="DeviceDataAuthInfoResponse"/>：device_list 为「容器 + item」包装形态，元素见 <see cref="DeviceDataAuthDevice"/>）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96097"/></para>
    /// <para>官方权限：第三方应用须具有「设备接口信息权限」授权；企业须同时安装硬件设备和第三方应用；
    /// 企业须向第三方应用二次授权具体硬件设备的原始数据。</para>
    /// <para>官方业务限制：企业自建应用与服务商代开发应用文档树无本端点页面（均不开放）；
    /// 官方契约陷阱：本端点容器内层元素名为 item（单数），而三条数据面容器内层元素名为 items，同域两种元素名。</para>
    /// </remarks>
    [Post("/cgi-bin/devicedata/get_auth_info")]
    Task<DeviceDataAuthInfoResponse> GetDeviceAuthInfoAsync(
        [Body] DeviceDataAuthInfoRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取考勤打卡原始数据
    /// <para>按时间区间与设备 / 成员范围分页获取设备上报的打卡数据。</para>
    /// <para>官方限制：请求时间跨度应在一个月内，且跨月请求易返回重复数据（官方要求尽量避免）；
    /// 返回数据按打卡记录上传时间排序；部分场景下可能返回重复数据，调用端应自行排重；
    /// <c>device_sn_list</c> 与 <c>open_userid_list</c> 各最多 100 个，成员列表须在应用可见范围否则被忽略；
    /// <c>limit</c> 不填默认 200、最多 500；<c>cursor</c> 由调用方保证正确性；
    /// <c>next_cursor</c> 非空表示可能仍有数据需继续调用、返回空字符串代表已是最后一页，
    /// 极端情况下因权限检查等筛选 <c>items</c> 可能为空但后续仍有满足条件的数据；
    /// 官方页面无独立的数值型频率限制，走官方全局访问频率限制。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="DeviceDataCheckinDataRequest"/>：user_type / begin_time / end_time / data_filter_type 必填，device_sn_list 与 open_userid_list 划定范围，cursor / limit 翻页）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>打卡数据列表（<see cref="DeviceDataCheckinDataResponse"/>：checkindata 容器 + next_cursor 翻页标记；条目时间字段官方名为 checkin_time）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96027"/></para>
    /// <para>官方权限：第三方应用须具有「设备接口信息权限 - 考勤打卡原始数据」授权；企业须同时安装硬件设备和第三方应用；
    /// 企业须向第三方应用二次授权具体硬件设备的原始数据；仅返回应用可见范围内成员的打卡数据。</para>
    /// <para>官方业务限制：企业自建应用与服务商代开发应用文档树无本端点页面（均不开放）；
    /// 三条数据面（打卡 / 温度检测 / 门禁通行）请求字段面同构，响应容器名分别为
    /// checkindata（连写无下划线）/ temperature_data / accesscontrol_data，条目时间字段名分别为 checkin_time 与 timestamp。</para>
    /// </remarks>
    [Post("/cgi-bin/devicedata/get_checkin_data")]
    Task<DeviceDataCheckinDataResponse> GetDeviceCheckinDataAsync(
        [Body] DeviceDataCheckinDataRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取温度检测原始数据
    /// <para>按时间区间与设备 / 成员范围分页获取设备上报的温度检测数据。</para>
    /// <para>官方限制：与打卡侧同一套官方约束（时间跨度应在一个月内且跨月易返回重复数据、按记录上传时间排序、
    /// 可能返回重复数据须调用端排重、两侧列表各最多 100 个、limit 默认 200 最多 500、
    /// next_cursor 非空表示仍有数据而空字符串代表最后一页且极端情况下条目可能为空）；
    /// 官方页面无独立的数值型频率限制，走官方全局访问频率限制。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="DeviceDataTemperatureDataRequest"/>：字段面同打卡侧，data_filter_type 取值为温度侧参数表口径）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>温度检测数据列表（<see cref="DeviceDataTemperatureDataResponse"/>：temperature_data 容器 + next_cursor；官方 temperature 以字符串承载而非数字）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96028"/></para>
    /// <para>官方权限：第三方应用须具有「设备接口信息权限 - 温度检测原始数据」授权；企业须同时安装硬件设备和第三方应用；
    /// 企业须向第三方应用二次授权具体硬件设备的原始数据；仅返回应用可见范围内成员的温度检测数据。</para>
    /// <para>官方业务限制：企业自建应用与服务商代开发应用文档树无本端点页面（均不开放）；
    /// 本端点条目同时回传 user_type（打卡侧条目无该字段），温度值与状态分别为字符串与数字枚举。</para>
    /// </remarks>
    [Post("/cgi-bin/devicedata/get_temperature_data")]
    Task<DeviceDataTemperatureDataResponse> GetDeviceTemperatureDataAsync(
        [Body] DeviceDataTemperatureDataRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 获取门禁通行原始数据
    /// <para>按时间区间与设备 / 成员范围分页获取设备上报的门禁通行数据。</para>
    /// <para>官方限制：与打卡侧同一套官方约束（时间跨度应在一个月内且跨月易返回重复数据、按记录上传时间排序、
    /// 可能返回重复数据须调用端排重、两侧列表各最多 100 个、limit 默认 200 最多 500、
    /// next_cursor 非空表示仍有数据而空字符串代表最后一页且极端情况下条目可能为空）；
    /// 官方页面无独立的数值型频率限制，走官方全局访问频率限制。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="DeviceDataAccessControlDataRequest"/>：字段面同打卡侧，data_filter_type 取值为门禁侧参数表口径）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>门禁通行数据列表（<see cref="DeviceDataAccessControlDataResponse"/>：accesscontrol_data 容器 + next_cursor，条目含 pass_type / pass_method 两个数字枚举）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96029"/></para>
    /// <para>官方权限：第三方应用须具有「设备接口信息权限 - 门禁通行原始数据」授权；企业须同时安装硬件设备和第三方应用；
    /// 企业须向第三方应用二次授权具体硬件设备的原始数据；仅返回应用可见范围内成员的门禁通行数据。</para>
    /// <para>官方枚举口径：条目 <c>user_type</c> 为 0-企业成员 / 1-访客 / 2-学生（请求侧参数表只给 0 与 2）；
    /// <c>pass_type</c> 为 1-进门 / 2-出门 / 3-无法识别方向；
    /// <c>pass_method</c> 为 1-人脸 / 2-二维码 / 3-远程开门 / 4-指纹。</para>
    /// <para>官方业务限制：企业自建应用与服务商代开发应用文档树无本端点页面（均不开放）；
    /// 本端点只读取通行记录，规则的读写走 get / add / mod / del_accesscontrol_rule 四端点。</para>
    /// </remarks>
    [Post("/cgi-bin/devicedata/get_accesscontrol_data")]
    Task<DeviceDataAccessControlDataResponse> GetDeviceAccessControlDataAsync(
        [Body] DeviceDataAccessControlDataRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 读取门禁通行规则
    /// <para>按单台设备序列号查询其门禁规则与远程门禁规则列表。</para>
    /// <para>官方限制：请求侧仅 <c>device_sn</c> 一个业务参数且为必填；官方页面无独立的数值型频率限制，走官方全局访问频率限制；
    /// 一个 <c>rule_id</c> 下可存在多条 <c>rule_list</c> 规则，其解析语法见官方「附录：门禁放行规则语法说明」。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="DeviceDataAccessControlRuleGetRequest"/>：仅 device_sn，官方为单值而非列表）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>门禁规则列表（<see cref="DeviceDataAccessControlRuleGetResponse"/>：pass_rule / remote_pass_rule 在响应侧为「容器 + items」形态，规则名官方字段为 name）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96030"/></para>
    /// <para>官方权限：第三方应用须具有「设备接口信息权限 - 读取门禁规则」授权（本端点为读侧独立权限，与增删改侧的「创建门禁规则」权限不同）；
    /// 企业须同时安装硬件设备和第三方应用；仅返回第三方应用可见范围内成员对应设备的门禁规则数据。</para>
    /// <para>官方业务限制：企业自建应用与服务商代开发应用文档树无本端点页面（均不开放）；
    /// 官方契约陷阱：pass_rule 在本端点响应里是列表容器、在添加 / 修改请求里是单对象，同名不同形；
    /// 规则名请求侧写 rule_name、响应侧读 name，且官方响应示例中存在不含 name 的规则项。</para>
    /// </remarks>
    [Post("/cgi-bin/devicedata/get_accesscontrol_rule")]
    Task<DeviceDataAccessControlRuleGetResponse> GetDeviceAccessControlRuleAsync(
        [Body] DeviceDataAccessControlRuleGetRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 写入门禁通行规则
    /// <para>为若干设备创建一条门禁规则（放行规则 + 生效成员），可同时配置远程门禁规则。</para>
    /// <para>官方限制：<c>device_sn_list</c> 与 <c>rule_name</c> 必填；<c>pass_rule</c> 与 <c>remote_pass_rule</c> 本身标「必须=否」，
    /// 但其子字段 <c>rule_list</c> 与 <c>effect_open_userid_list</c> 均标「必须=是」（生效成员内的 <c>open_userid</c> 同为必填、
    /// <c>user_type</c> 选填），即<b>传对象就得带齐子字段</b>；官方原文「调用此接口，至少增加一条门禁规则或远程开门规则，否则会报错」；
    /// <c>pass_rule</c> 目前仅第一（时分）与第四（星期）字段生效（共 5 个字段），
    /// 其余星号被替换成其他内容暂不生效，且暂不支持 <c>/</c> 操作符；本页面未给出独立的数值型频率限制，走官方全局访问频率限制（默认阈值见本接口 <c>remarks</c>）。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="DeviceDataAccessControlRuleAddRequest"/>：rule_name / device_sn_list 必填，pass_rule 与 remote_pass_rule 为单对象形态的 <see cref="DeviceDataPassRule"/>）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>新建规则的 rule_id 与非法成员列表（官方 invalid_list，参数表口径为「errcode 不为 0 时此字段有效」）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96031"/></para>
    /// <para>官方权限：第三方应用须具有「设备接口信息权限 - 创建门禁规则」授权；企业须同时安装硬件设备和第三方应用；
    /// 仅能对「第三方应用可见范围内成员」与「授权设备可见范围内成员」的交集成员创建规则；仅能对授权给本应用的设备列表范围内的设备创建规则。</para>
    /// <para>官方业务限制：企业自建应用与服务商代开发应用文档树无本端点页面（均不开放）；
    /// 成员以 OpenUserId 表达（本域全线使用 open_userid 而非 userid），<c>user_type</c> 为 0-企业员工 / 2-学生，不填默认 0。</para>
    /// </remarks>
    [Post("/cgi-bin/devicedata/add_accesscontrol_rule")]
    Task<DeviceDataAccessControlRuleAddResponse> AddDeviceAccessControlRuleAsync(
        [Body] DeviceDataAccessControlRuleAddRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 修改门禁通行规则
    /// <para>按规则 ID 覆盖修改门禁规则的名称、绑定设备与放行 / 远程放行配置。</para>
    /// <para>官方限制：<c>rule_id</c>（企业微信返回的规则标识）、<c>device_sn_list</c>、<c>rule_name</c> 均必填；
    /// <c>pass_rule</c> 与添加侧同样仅第一（时分）与第四（星期）字段生效、暂不支持 <c>/</c> 操作符；
    /// 官方页面无独立的数值型频率限制，走官方全局访问频率限制。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="DeviceDataAccessControlRuleModRequest"/>：rule_id / rule_name / device_sn_list 必填，pass_rule 与 remote_pass_rule 为单对象形态）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>非法成员列表（<see cref="DeviceDataAccessControlRuleModResponse"/>：官方 invalid_list，参数表口径为「errcode 不为 0 时此字段有效」；本端点不回传 rule_id）。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96221"/></para>
    /// <para>官方权限：第三方应用须具有「设备接口信息权限 - 创建门禁规则」授权（与添加侧共用同一权限，官方未设独立修改权限）；
    /// 企业须同时安装硬件设备和第三方应用；修改规则的成员只能包含「应用可见成员」与「授权设备可见成员」的交集成员；
    /// 若已存在的规则包含该交集外的成员、或包含本应用授权设备列表范围外的设备，则无法经本端点修改；
    /// <b>第三方应用只能更改自己创建的门禁规则</b>。</para>
    /// <para>官方业务限制：企业自建应用与服务商代开发应用文档树无本端点页面（均不开放）；
    /// 本端点为整体覆盖语义（请求即新的完整规则），与添加侧的差异在于额外以 rule_id 定位且不回传 rule_id。</para>
    /// </remarks>
    [Post("/cgi-bin/devicedata/mod_accesscontrol_rule")]
    Task<DeviceDataAccessControlRuleModResponse> ModifyDeviceAccessControlRuleAsync(
        [Body] DeviceDataAccessControlRuleModRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// 删除门禁通行规则
    /// <para>按规则 ID 删除门禁规则。</para>
    /// <para>官方限制：请求侧仅 <c>rule_id</c>（企业微信返回的门禁规则标识 id）一个业务参数；
    /// 官方页面无独立的数值型频率限制，走官方全局访问频率限制。</para>
    /// </summary>
    /// <param name="request">请求体（<see cref="DeviceDataAccessControlRuleDelRequest"/>：仅 rule_id）。</param>
    /// <param name="cancellationToken"><see cref="CancellationToken"/>取消操作令牌对象。</param>
    /// <returns>官方成功响应仅有 errcode / errmsg，无业务字段。</returns>
    /// <remarks>
    /// <para><b>第三方应用</b>开发SDK文档：<see href="https://developer.work.weixin.qq.com/document/path/96227"/></para>
    /// <para>官方权限：第三方应用须具有「设备接口信息权限 - 创建门禁规则」授权（与添加 / 修改侧共用同一权限）；
    /// 企业须同时安装硬件设备和第三方应用；<b>第三方应用只能删除自己创建的门禁规则</b>。</para>
    /// <para>官方业务限制：企业自建应用与服务商代开发应用文档树无本端点页面（均不开放）；
    /// 官方响应体不含业务字段，故返回 <see cref="WechatWorkResponse"/>，不另建响应 DTO。</para>
    /// </remarks>
    [Post("/cgi-bin/devicedata/del_accesscontrol_rule")]
    Task<WechatWorkResponse> DeleteDeviceAccessControlRuleAsync(
        [Body] DeviceDataAccessControlRuleDelRequest request,
        CancellationToken cancellationToken = default);
}
