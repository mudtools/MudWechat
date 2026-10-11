// -----------------------------------------------------------------------
//  作者：Mud Studio  版权所有 (c) Mud Studio 2026
//  Mud.Wechat 项目的版权、商标、专利和其他相关权利均受相应法律法规的保护。
//  本项目主要遵循 MIT 许可证进行分发和使用。许可证位于源代码树根目录中的 LICENSE-MIT 文件。
//  不得利用本项目从事危害国家安全、扰乱社会秩序、侵犯他人合法权益等法律法规禁止的活动！任何基于本项目开发而产生的一切法律纠纷和责任，我们不承担任何责任！
// -----------------------------------------------------------------------

namespace Mud.Wechat.Ads.Tests.ContractGuards;

/// <summary>
/// 腾讯广告（Marketing API v3.0）产品线契约守卫（方案 §3.5 ADS-B 系列的机械化落地）。
/// </summary>
/// <remarks>
/// <para>
/// <b>本文件的分工</b>：<c>ADS-S1/S2</c> 锁依赖边界与 TFM 口径（「新产品线接进门禁」的实质内容），
/// <c>ADS-B1</c> 锁凭据注入形态，<c>ADS-B4</c> 锁 SSRF 白名单并集口径，<c>ADS-B6</c> 锁 AOT 净零与
/// 上下文登记完整性；<c>ADS-B2</c> 逐路由 / 逐参数名 / 逐字段名镜像官方原文，<c>ADS-B3</c> 锁
/// refresh_token 的一次性语义与失败顺序，<c>ADS-B5</c> 锁 Query 凭据的脱敏覆盖面。
/// <b>每条都是实质断言，不留空断言</b>（空断言 = 假绿）。
/// </para>
/// <para>
/// <b>为何广告线需要独立守卫族</b>：它是本仓第一条<b>非微信域</b>产品线（<c>api.e.qq.com</c>），
/// 响应信封是 <c>{code, message, message_cn, data}</c> 而非 <c>errcode/errmsg</c>，且凭据是
/// OAuth2 双令牌（access + 一次性 refresh）。任何一条与微信线共用口径的假设都不成立。
/// </para>
/// <para>
/// <b>两种取证形态按「断言的性质」选择</b>：路由 / 参数名 / 字段名 / 注册组用<b>反射</b>（契约的真实形状，
/// 改名即红）；「谁在什么顺序上做了什么」这类<b>过程性</b>约束（ADS-B3 的先删后抛、ADS-B6 的登记面）
/// 反射拿不到，用<b>剔注释后的源码文本</b>断言。文本断言必须配合行为用例（<c>AdsAuthorizationServiceTests</c>
/// 的 <c>*_ShouldRemoveState*</c>）—— 文本抓「新增路径忘了做」，行为抓「做了但顺序错」。
/// </para>
/// </remarks>
public class AdsContractGuards
{
    /// <summary>
    /// ADS-S1：<b>依赖边界</b> —— 广告线只依赖公用层叶包与自身三包，<b>不得</b>引用任何其它产品线；
    /// 反向同理，其它产品线也<b>不得</b>引用广告线。
    /// </summary>
    /// <remarks>
    /// 广告线与公众号/小程序/支付线没有任何共享令牌域（v3.0 是独立的 OAuth2 授权体系），
    /// 跨线引用只会造成「凭据模型混用」这一类静默缺陷（例如把 <c>Wechat.Mp.AccessToken</c>
    /// 的 Query 注入形态套到 Ads，官方直接拒）。硬边界与 AGENTS §4 的单向依赖同一口径。
    /// <para>
    /// <b>两个方向都要扫</b>：正向（Ads → 其它线）由逐条引用名判定；反向（其它线 → Ads）若不做断言，
    /// 某人把 <c>IAdsTokenManager</c> 注进 Work 包就能绕过正向断言，而包依赖图仍是「单向」的。
    /// </para>
    /// </remarks>
    [Fact]
    public void AdsLine_ShouldDependOnlyOnCoreAndOwnPackages()
    {
        foreach (var project in new[]
                 {
                     "Mud.Wechat.Ads",
                     "Mud.Wechat.Ads.Abstractions",
                     "Mud.Wechat.Ads.DataModels",
                 })
        {
            var content = File.ReadAllText(
                Path.Combine(SourceProjectDir(project), project + ".csproj"));

            // 逐条判定：本包引用「其它产品线工程」即违规（自身三包互引合法）。
            foreach (var refName in new[]
                     {
                         "Mud.Wechat.Work", "Mud.Wechat.Work.Abstractions", "Mud.Wechat.Work.Callback",
                         "Mud.Wechat.OfficialAccount", "Mud.Wechat.OfficialAccount.Abstractions",
                         "Mud.Wechat.MiniProgram", "Mud.Wechat.MiniProgram.Abstractions",
                         "Mud.Wechat.Pay", "Mud.Wechat.Pay.Abstractions",
                         "Mud.Wechat.OpenPlatform", "Mud.Wechat.OpenPlatform.Abstractions",
                         // 微信小店 / 视频号（channels 生态，2026-10 并入本仓）：与广告线无共享凭据域，同样禁引用。
                         "Mud.Wechat.Channels", "Mud.Wechat.Channels.Abstractions", "Mud.Wechat.Channels.Callback",
                         "Mud.Wechat.Redis",
                     })
            {
                content.Should().NotContain($"{refName}.csproj",
                    $"广告线 {project} 与 {refName} 无共享凭据域，跨线引用即凭据模型混用风险（ADS-S1）");
            }
        }

        // 反向：Src/ 下所有非广告线工程都不得引用广告线三包。
        var adsNames = new[] { "Mud.Wechat.Ads.csproj", "Mud.Wechat.Ads.Abstractions.csproj", "Mud.Wechat.Ads.DataModels.csproj" };
        var foreignHits = Directory
            .EnumerateFiles(Path.Combine(Root, "Src"), "*.csproj", SearchOption.AllDirectories)
            .Where(static f => !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                               && !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .Where(f => !f.Contains($"{Path.DirectorySeparatorChar}Ads{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .Where(f => adsNames.Any(n => File.ReadAllText(f).Contains(n, StringComparison.Ordinal)))
            .Select(f => Path.GetRelativePath(Root, f).Replace('\\', '/'))
            .ToArray();

        foreignHits.Should().BeEmpty(
            "广告线凭据模型（OAuth 双令牌 + 成组注入）与微信线不兼容，其它产品线引用它即把该不兼容" +
            $"带进自己的调用链（ADS-S1 反向）；实际命中：{string.Join(", ", foreignHits)}");
    }

    /// <summary>
    /// ADS-S2：<b>TFM 口径</b> —— 广告线四档 TFM 全部继承根 props，<b>不得</b>自定 <c>TargetFrameworks</c>。
    /// </summary>
    /// <remarks>
    /// 支付线的 <c>net6.0;net8.0;net10.0</c> 是因 <c>AesGcm</c> 在 netstandard2.0 不存在的<b>受控例外</b>、
    /// 开放平台线是刻意收窄到组件最小面的<b>受控例外</b>（两者均由守卫 AB-G8 锁定并逐工程排除在外）。
    /// 广告线只做 HTTPS + JSON，无原生密码学依赖 ⇒ 不得援引该例外，
    /// 否则「例外外溢」会让 netstandard2.0 宿主静默失去广告线引用能力。
    /// </remarks>
    [Fact]
    public void AdsLine_ShouldInheritFourTfmBaseline()
    {
        foreach (var project in new[]
                 {
                     "Mud.Wechat.Ads",
                     "Mud.Wechat.Ads.Abstractions",
                     "Mud.Wechat.Ads.DataModels",
                 })
        {
            var content = File.ReadAllText(
                Path.Combine(SourceProjectDir(project), project + ".csproj"));

            content.Should().NotContain("<TargetFrameworks>",
                $"{project} 必须继承根 props 的四档 TFM（受控 TFM 例外只属于支付线，AB-G8）");
        }
    }

    /// <summary>
    /// ADS-B1：广告线<b>不得</b>声明 <c>[Token]</c> —— 凭据由 OAuth 令牌提供、在传输层注入。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 与支付线 PAY-B1 同一形态的 fail-closed 断言，但理由不同：支付线是<b>根本没有</b> access_token；
    /// 广告线<b>有</b> access_token，却<b>不能</b>用声明式 <c>[Token]</c> —— v3.0 要求每个业务请求
    /// 同时携带 <c>access_token</c> + <c>timestamp</c> + <c>nonce</c>（官方「全局参数」表，
    /// 时间戳误差上限 300 秒、nonce 全局唯一 ≤32 字符）。三个参数必须<b>成组且每次现取</b>，
    /// 声明式注入只能给其一 ⇒ 组装不出合法请求。故令牌面收敛到唯一咽喉点
    /// （<c>AdsAuthorizationHandler</c>），端点方法只管路由与报文。
    /// </para>
    /// <para><b>扫描形态</b>：源码文本扫描而非反射 —— 反射需等接口成形，而本守卫的价值恰在
    /// 「实现落地前就把注入形态钉住」。文本扫描对 <c>[Token(</c> 精确匹配，注释里的 <c>[Token]</c> 不误报。</para>
    /// </remarks>
    [Fact]
    public void AdsInterfaces_ShouldNotDeclareTokenAttribute()
    {
        var hits = AdsSourceFiles()
            .Where(f => File.ReadAllText(f).Contains("[Token(", StringComparison.Ordinal))
            .Select(Path.GetFileName)
            .ToArray();

        hits.Should().BeEmpty(
            "广告线凭据必须在传输层成组注入 access_token/timestamp/nonce，不得用声明式 [Token]（ADS-B1）；" +
            $"实际命中：{string.Join(", ", hits)}");
    }

    /// <summary>
    /// ADS-B4：<b>SSRF 白名单并集追加</b> —— <c>e.qq.com</c> 已在公用层单点登记，
    /// 且广告线<b>不得</b>自行调用 <c>ConfigureAllowedDomains</c> 或塞入完整主机名条目。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 组件 <c>UrlValidator.ConfigureAllowedDomains</c> 是<b>进程级全局静态 + 整体替换</b>语义
    /// （调用后白名单恰好等于传入集合）。微信系四条产品线都能靠 <c>weixin.qq.com</c> 后缀覆盖，
    /// 广告线的 <c>api.e.qq.com</c> <b>覆盖不到</b> ⇒ 必须新增条目，而新增即扩大全局放行面，
    /// 所以本守卫把「只加一条后缀域、不加 FQDN」钉死：
    /// </para>
    /// <list type="bullet">
    /// <item><description><c>e.qq.com</c> 必须存在（缺 ⇒ 广告线每笔请求被连接期 SSRF 严格模式拦下）。</description></item>
    /// <item><description><c>api.e.qq.com</c> 等业务主机<b>不得</b>作为独立条目出现（后缀项已覆盖，
    /// 逐主机登记会让放行面随端点增长而失控）。</description></item>
    /// <item><description>登记点仍只有公用层一处（AB-G4 同口径，本守卫只补广告线侧的「不得自行登记」）。</description></item>
    /// </list>
    /// </remarks>
    [Fact]
    public void SsrfWhitelist_ShouldCoverAdsHostBySuffixAppendOnly()
    {
        WechatApiHosts.AllowedBaseUrlDomains.Should().Contain("e.qq.com",
            "api.e.qq.com 不在 weixin.qq.com 后缀覆盖内，广告线必须以 e.qq.com 一条后缀域接入白名单并集");

        foreach (var fqdn in new[] { "api.e.qq.com", "developers.e.qq.com" })
        {
            WechatApiHosts.AllowedBaseUrlDomains.Should().NotContain(fqdn,
                $"{fqdn} 已被 e.qq.com 后缀覆盖（判定语义：host 等于域或以其子域结尾即放行），" +
                "逐主机登记会随端点增长扩大全局放行面");
        }

        // 白名单是数组字面量，条目重复不会报错但会让「并集」语义变得不可审计。
        WechatApiHosts.AllowedBaseUrlDomains
            .GroupBy(static d => d, StringComparer.OrdinalIgnoreCase)
            .Where(static g => g.Count() > 1)
            .Select(static g => g.Key)
            .Should().BeEmpty("白名单条目必须唯一，重复条目说明并集追加走了复制粘贴而非单点定义");

        var hits = AdsSourceFiles()
            .Where(f => File.ReadAllText(f).Contains("ConfigureAllowedDomains", StringComparison.Ordinal))
            .Select(Path.GetFileName)
            .ToArray();

        hits.Should().BeEmpty(
            "SSRF 白名单登记只能出现在公用层单点（WechatTokenRecoveryRegistration），" +
            $"广告线不得自行登记（整体替换语义会清空其它产品线）；实际命中：{string.Join(", ", hits)}");
    }

    /// <summary>
    /// ADS-B6：<b>AOT 净零</b> —— 广告线源码不得出现反射版 <c>JsonSerializer</c> 读写。
    /// </summary>
    /// <remarks>
    /// 与支付线同形态：Native AOT 下反射重载没有元数据 ⇒ JIT 一路正常、AOT 首次调用即失败，
    /// 属最难复现的一类缺陷。广告线 DTO 一律标 <c>[HttpJsonSerializable]</c> 并生成域 JsonContext，
    /// 经 <c>AdsJsonResolverExtensions</c> 合并进组件解析器。
    /// <para><b>必须剔除注释再计数</b>：本仓多处 XML 注释会<b>解释</b>「为什么禁用反射版
    /// <c>JsonSerializer.Serialize</c>」，直接子串匹配会对文档误报（AB-G9 已踩过一次）。</para>
    /// </remarks>
    [Fact]
    public void AdsSources_ShouldNotUseReflectionJson()
    {
        var hits = AdsSourceFiles()
            .Select(f => new { File = Path.GetFileName(f), Code = StripComments(File.ReadAllText(f)) })
            .Where(x => x.Code.Contains("JsonSerializer.Serialize(", StringComparison.Ordinal)
                        || x.Code.Contains("JsonSerializer.Deserialize(", StringComparison.Ordinal))
            .Select(x => x.File)
            .ToArray();

        hits.Should().BeEmpty(
            "AOT 下反射版 JsonSerializer 无元数据，必须走源生成 JsonContext（ADS-B6）；" +
            $"实际命中：{string.Join(", ", hits)}");
    }

    /// <summary>
    /// ADS-B5：<b>Query 承载凭据的参数名</b>必须「被组件静态词表覆盖」或「被本线显式登记」，二者恰居其一。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>三条口径</b>（与企微线 G7 同源，判定面取广告线<b>全部</b>会进 Query 的凭据名）：
    /// ① 覆盖面 = 反射到的业务接口 <c>[Query]</c> 名 ∪ OAuth 手写 <c>BuildUri</c> 实参名 ∪ 传输层注入名；
    /// ② 每个凭据名必须命中「词表 ∪ <see cref="AdsSensitiveQueryKeys.All"/>」之一，否则随
    /// <c>ApiException.RequestUri</c> / 遥测 URL 明文外泄；
    /// ③ 两集<b>不得交叉</b> —— 已在词表内的键再登记，会让「哪些靠词表、哪些靠登记」的分界失去可审计性，
    /// 这也是「组件词表一旦补齐、本线登记必须同批撤销」的<b>自过期</b>闸（与 G7 的豁免自过期同一机制）。
    /// </para>
    /// <para>
    /// <b><c>timestamp</c> / <c>nonce</c> 不参与本判定</b>：它们是「时效」与「唯一性」参数、不是秘密，
    /// 泄露它们不构成凭据外泄（官方对 timestamp 允许 300 秒误差、nonce 只要求全局唯一）。
    /// </para>
    /// </remarks>
    [Fact]
    public void AdsCredentialQueryParameters_ShouldBeRedactedByVocabularyOrRegistration()
    {
        var vocabulary = ReadComponentSensitiveVocabulary();
        vocabulary.Should().NotBeEmpty("未能读取组件脱敏词表（组件版本或 SensitiveFieldNames 变更，请同步本守卫）");

        var credentials = AdsCredentialQueryParameterNames();
        credentials.Should().Contain(new[]
            {
                "access_token", "refresh_token", "client_secret", "authorization_code", "user_token",
            },
            "广告线凭据面至少含 OAuth 双令牌 + client_secret + 一次性授权码 + 受限接口的实名认证令牌");

        var uncovered = credentials
            .Where(n => !ContainsIgnoreCase(vocabulary, n) && !ContainsIgnoreCase(AdsSensitiveQueryKeys.All, n))
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToArray();

        uncovered.Should().BeEmpty(
            "以下 Query 参数承载凭据、既不在组件词表也不在本线登记表：会明文出现在异常 RequestUri / 遥测 URL。" +
            $"请二选一（勿用第三条路）：推动组件补词表，或在 AdsSensitiveQueryKeys.All 登记并在此说明理由：{string.Join(", ", uncovered)}");

        // 词表侧的四支必须真的在词表里 —— 若组件回退词表，本条先红，而不是让本线登记面「看起来兜住了」。
        foreach (var name in new[] { "access_token", "refresh_token", "client_secret", "authorization_code" })
        {
            vocabulary.Should().Contain(name,
                $"{name} 属官方强制 Query 承载的凭据，必须由组件词表覆盖（本线不重复登记）");
        }

        var overlap = AdsSensitiveQueryKeys.All
            .Where(n => ContainsIgnoreCase(vocabulary, n))
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToArray();

        overlap.Should().BeEmpty(
            "登记面与组件词表交叉 ⇒ 组件已覆盖该键，本线登记已过期，必须从 AdsSensitiveQueryKeys.All 移除" +
            $"（保留会让真实缺口被冗余条目掩盖）：{string.Join(", ", overlap)}");

        // 登记面不得「登记不存在的键」：过度宽泛的键名会扩大全局脱敏面、妨碍排障（组件文档明确警告）。
        var declared = AdsAllQueryParamNames();
        var orphans = AdsSensitiveQueryKeys.All
            .Where(n => !ContainsIgnoreCase(declared, n))
            .OrderBy(n => n, StringComparer.Ordinal)
            .ToArray();

        orphans.Should().BeEmpty(
            $"以下登记键在广告线任何 Query 参数面上都不存在，属无根据的全局脱敏扩张：{string.Join(", ", orphans)}");
    }

    /// <summary>
    /// ADS-B2（<b>首批端点路由表</b>）：<c>advertiser</c> 域 3 端点的<b>方法 × 路由 × HTTP 动词</b>逐条等于官方原文。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 官方 <c>advertiser/*</c> 恰有 <c>get</c> / <c>update</c> / <c>update_daily_budget</c> 三支
    /// （2026-10-10 逐页核验）：<c>get</c> 为 <b>GET</b>（curl 用 <c>-G -d</c> ⇒ 参数进 Query、无请求体），
    /// 另两支为 <b>POST</b> + <c>Content-Type: application/json</c>。
    /// </para>
    /// <para>
    /// <b>为什么只能是硬编码逐条断言</b>：动词或路由写错<b>不会</b>被编译期或组件运行期任何检查抓到 ——
    /// 组件按声明原样发送，官方网关回的是「参数缺失 / 资源不存在」而不是「方法不允许」，
    /// 现象与「账号没权限」难以区分。故路由表必须写死在守卫里，作为官方契约的镜像。
    /// </para>
    /// </remarks>
    [Fact]
    public void AdvertiserEndpoints_ShouldMatchOfficialRouteTable()
    {
        AdvertiserRoutes.Should().HaveCount(3, "官方 advertiser/* 恰 3 个端点，本域全覆盖且不多做");
        AdvertiserRoutes.Select(static r => r.Route).Distinct().Should().HaveCount(3, "路由互不重复");

        typeof(IWechatAdsAdvertiserService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().HaveCount(3, "接口端点数与守卫路由表条目数必须一致（新增端点未入表即红，防「实现了但没锁」）");

        foreach (var (iface, method, httpAttribute, route) in AdvertiserRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明与官方一致的 HTTP 动词");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约逐字符一致");
        }
    }

    /// <summary>
    /// ADS-B2（<b>adgroups 域路由表</b>）：营销单元域 8 端点的<b>方法 × 路由 × HTTP 动词</b>逐条等于官方原文。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 2026-10-10 逐页核验，官方 <c>adgroups/*</c> 恰有 <c>get</c> / <c>add</c> / <c>update</c> /
    /// <c>delete</c> / <c>update_daily_budget</c> / <c>update_configured_status</c> /
    /// <c>update_bid_amount</c> / <c>update_datetime</c> 八支：<b>只有 <c>get</c> 是 GET</b>，
    /// 其余七支均为 <b>POST</b> + <c>Content-Type: application/json</c>（与 <c>advertiser</c> 域同一形态，
    /// 也与本仓「多数写操作官方即 POST」的既存口径一致）。
    /// </para>
    /// <para>
    /// <b>批量支命名不可缩写</b>：<c>update_configured_status</c> / <c>update_bid_amount</c> /
    /// <c>update_datetime</c> 三支的路由尾段与其请求数组名（守卫 <see cref="AdsDataModelJsonNames_ShouldStayOfficialSnakeCase"/>
    /// 锁 <c>update_*_spec</c>）成对出现，把 <c>update_datetime</c> 写成 <c>update_date</c>
    /// 是官方<b>本页使用说明里的笔误</b>（原文误写 <c>update_date_spec</c>）—— 报文取请求表、路由取「请求地址」，
    /// 两处都不得跟着误写走，故钉成逐字符断言。
    /// </para>
    /// </remarks>
    [Fact]
    public void AdgroupsEndpoints_ShouldMatchOfficialRouteTable()
    {
        AdgroupsRoutes.Should().HaveCount(8, "官方 adgroups/* 恰 8 个端点，本域全覆盖且不多做");
        AdgroupsRoutes.Select(static r => r.Route).Distinct().Should().HaveCount(8, "路由互不重复");
        AdgroupsRoutes.Select(static r => r.Interface).Distinct().Should().HaveCount(1,
            "本表只描述 adgroups 一个资源族（混入它域路由会让端点计数失去意义）");

        typeof(IWechatAdsAdgroupService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().HaveCount(8, "接口端点数与守卫路由表条目数必须一致（新增端点未入表即红，防「实现了但没锁」）");

        foreach (var (iface, method, httpAttribute, route) in AdgroupsRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明与官方一致的 HTTP 动词");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约逐字符一致");
        }
    }

    /// <summary>
    /// ADS-B2（<b>官方资源族封闭性</b>）：全部已声明路由必须与守卫各域路由表<b>双向相等</b>、
    /// 一律带 <c>/v3.0/</c> 前缀，且<b>不得</b>出现 v3.0 已移除的资源族。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>v3.0 层级事实</b>（官方接口清单逐页核验 2026-10-10）：资源层级为
    /// <c>advertiser</c> → <c>adgroups</c> → <c>dynamic_creatives</c> → <c>components</c>；
    /// <c>campaigns/*</c>（推广计划层）与 <c>ads/*</c>（广告实例）在 v3.0 <b>不存在</b> ⇒ 本 SDK 不建模那两层。
    /// 该断言按<b>路径段</b>判别而非子串：<c>/v3.0/adgroups/get</c> 含 <c>ads</c> 子串但不是 <c>ads</c> 段，
    /// 用子串匹配会把本域正常路由判成违规（反之，写成 <c>/v3.0/campaigns/get</c> 一定会被抓）。
    /// </para>
    /// <para>
    /// <b>双向相等是「未落地守卫点名可见」的机制</b>：反射面 ⊆ 守卫表 抓「新增了端点却没进官方契约表」
    /// （实现先行、守卫后补的半成品）；守卫表 ⊆ 反射面 抓「表里有条目但接口已删」。
    /// 有了这一对，各域守卫里的 <c>HaveCount(n)</c> 才不会被「另一个域偷偷加端点」绕过。
    /// </para>
    /// </remarks>
    [Fact]
    public void AdsRoutes_ShouldEqualGuardTablesAndNeverReviveRemovedResources()
    {
        var reflected = AdsReflectedRoutes();
        reflected.Should().NotBeEmpty();

        var guarded = AdsGuardedRoutes()
            .Select(static r => Key(r.Interface.Name, r.Method, r.HttpAttribute.Name, r.Route))
            .OrderBy(static k => k, StringComparer.Ordinal)
            .ToArray();

        var actual = reflected
            .Select(static r => Key(r.Interface, r.Method, r.Verb, r.Route))
            .OrderBy(static k => k, StringComparer.Ordinal)
            .ToArray();

        actual.Should().Equal(guarded,
            "接口反射面与守卫路由表必须逐条（接口 × 方法 × 动词 × 路由）相等：" +
            "官方契约的镜像只认逐条硬编码，任何一侧单独变动都说明契约面与守卫脱节（AGENTS §2）");

        foreach (var route in actual.Select(static k => k.Split('|')[3]))
        {
            route.Should().StartWith("/v3.0/", "业务端点路由必须带官方版本前缀（基址只到主机名）");
        }

        foreach (var (route, segments) in actual.Select(static k => (k.Split('|')[3], k.Split('|')[3].Split('/', StringSplitOptions.RemoveEmptyEntries))))
        {
            segments.Should().NotContain(static s => RemovedV3Resources.Contains(s),
                $"{route} 落在 v3.0 已移除的资源族上：campaigns / ads 两支在 v3.0 清单里不存在，" +
                "对齐它们等于对接已下线的旧 API");
        }

        // 每条路由的动词只能是 GET / POST 两支之一（v3.0 无 PUT / DELETE / PATCH 形态）。
        reflected.Select(static r => r.Verb).Distinct().OrderBy(static v => v, StringComparer.Ordinal).Should()
            .Equal(new[] { "GetAttribute", "PostAttribute" },
                "官方 v3.0 业务端点只有 GET（查询）与 POST（写与批量）两种动词");
    }

    /// <summary>
    /// ADS-B2 / ADS-B5（<b><c>user_token</c> 只在 POST 面、<c>X-Request-Id</c> 只在 add</b>）：
    /// 两支「按页而异」的请求参数面逐方法锁定。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b><c>user_token</c></b>（实名认证令牌，官方「特定请求参数」表）：adgroups 的 7 支 POST 页都列出它、
    /// <c>get</c> 页<b>没有</b>该表 ⇒ 「GET 面不带」是官方事实而非实现选择。若给 GET 也加上，
    /// 官方按 Query 解析未声明参数会把令牌留在 URL 里白白多一处外泄面（ADS-B5 的登记面只管已声明的形态，
    /// 管不住「不该带的参数」，故须由本条按方法逐个点名）。
    /// </para>
    /// <para>
    /// <b>官方原文「注意不是放在 header 中」</b>：这条把 <c>user_token</c> 的载体钉成 Query，
    /// 常见「更安全」的直觉（挪进 header）在此<b>是错的</b> —— 官方不读 header 里的该参数。
    /// </para>
    /// <para>
    /// <b><c>X-Request-Id</c></b>：v3.0 全部已核验页面里<b>只有 <c>adgroups/add</c></b> 有幂等请求头表
    /// （官方原文：重复请求「API 侧永远不会新建一个全新的投放资源」）。
    /// <c>update</c> / <c>delete</c> 与四支批量页均无 ⇒ 「给所有写端点都加个幂等 id」看着整齐，
    /// 实则是发送官方未定义的请求头并让人误以为重试安全。
    /// </para>
    /// </remarks>
    [Fact]
    public void AdgroupsQueryCredentialAndIdempotencyHeader_ShouldMatchPerPageOfficialTables()
    {
        var methods = typeof(IWechatAdsAdgroupService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .ToArray();

        // ① user_token 的承载面：恰为 7 支非 GET 方法，逐一点名（不用「除 GET 外全部」这种会随新增域漂移的相对表述）。
        var postSurface = methods
            .Where(static m => m.GetCustomAttribute<GetAttribute>() is null)
            .Select(static m => m.Name)
            .OrderBy(static n => n, StringComparer.Ordinal)
            .ToArray();

        postSurface.Should().HaveCount(7, "官方 adgroups 域 8 支端点中 7 支为 POST");

        var carryingUserToken = methods
            .Where(static m => QueryParameterNames(m).Contains("user_token"))
            .Select(static m => m.Name)
            .OrderBy(static n => n, StringComparer.Ordinal)
            .ToArray();

        carryingUserToken.Should().Equal(postSurface,
            "user_token 必须逐 POST 方法在场且缺席于 get（官方 get 页无「特定请求参数」表）；" +
            "少一处 = 受限接口调用直接被拒，多一处 = 凭据无谓进 URL");

        foreach (var method in methods.Where(static m => m.GetCustomAttribute<GetAttribute>() is null))
        {
            QueryParameterNames(method).Count(static n => n == "user_token").Should().Be(1,
                $"{method.Name} 上的 user_token 必须恰好一个（重复声明会让同一请求携带两份不同令牌）");
        }

        typeof(IWechatAdsAdgroupService)
            .GetMethod(nameof(IWechatAdsAdgroupService.GetAsync), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!
            .GetParameters()
            .Select(static p => p.GetCustomAttribute<QueryAttribute>()?.Name)
            .Should().NotContain(static n => n == "user_token",
                "官方 get 页未列出 user_token ⇒ 不得因「与写端点保持一致」而添加");

        // ② 载体必须是 Query 而非 Header（官方原文「注意不是放在 header 中」）。
        var headerNames = AdsHeaderParameterNames();
        headerNames.Select(static h => h.Name).Distinct().OrderBy(static n => n, StringComparer.Ordinal).Should()
            .Equal(new[] { "X-Request-Id" },
                "v3.0 已核验页面里只有 adgroups/add 有请求头参数表 ⇒ 出现第二个头名即官方未定义");

        headerNames.Select(static h => h.Owner).Distinct().Should()
            .Equal(new[] { "IWechatAdsAdgroupService.AddAsync" },
                "幂等头只属于 add 一支：update / delete / 批量页均无该表，套到全部写端点会给出虚假的重试安全性");

        // user_token 走 Query 而非 Header —— 头名集合不含它即证明承载形态未漂。
        headerNames.Should().NotContain(static h => h.Name == "user_token",
            "官方原文明令 user_token 不放在 header 中（放 header 官方读不到）");

        // ③ 与 ADS-B5 的衔接口径：本域带来的新凭据名必须已被登记（不是由本条重复断言词表）。
        AdsAllQueryParamNames().Should().Contain("user_token",
            "user_token 的脱敏登记面（AdsSensitiveQueryKeys）反查依据即此参数名集合");
    }

    /// <summary>
    /// ADS-B2（<b>字段所在层级 = 官方文档 DOM 层级</b>）：字段名对了但<b>挂错父结构</b>同样静默失效。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>本条存在的全部理由</b>：2026-10-10 逐页核验时，平面文本抽取（innerText / textContent）把三处
    /// <c>list[]</c> 顶层字段读成了兄弟 <c>struct</c> 的子字段 —— <c>targeting_translation</c> 被归入
    /// <c>targeting</c>、<c>poi_list</c> 被归入 <c>marketing_asset_outer_spec</c>。只有按官方页面的
    /// <c>level-*</c> class（DOM 缩进层级）重读才确认它们是<b>顶层</b>字段；反过来
    /// <c>iaa_smart_hosting</c> / <c>high_volume_exploration</c> / <c>prospect_retargeting</c> 三支
    /// <b>确在</b> <c>industry_value_explore</c> 之内。层级写错的后果与漏标 <c>snake_case</c> 完全同型：
    /// 名字合法、序列化不报错、官方应答里该键因父节点不匹配而<b>静默绑不上</b>。
    /// 故「名字面」的守卫（<see cref="AdsDataModelJsonNames_ShouldStayOfficialSnakeCase"/>）覆盖不到，
    /// 必须按<b>类型 × 名字</b>成对断言。
    /// </para>
    /// <para>
    /// <b>另一类同型风险：跟着官方笔误走</b>。<c>update_datetime</c> 页的使用说明把数组名写成
    /// <c>update_date_spec</c>（请求参数表为 <c>update_datetime_spec</c>）⇒ 报文权威取请求表，
    /// 全仓不得出现笔误形态。
    /// </para>
    /// <para>
    /// <b>第三类：<c>update</c> 面不等于 <c>add</c> 面</b>。官方 <c>adgroups/update</c> 的参数表比
    /// <c>add</c> 少约二十三支（<c>marketing_goal</c> / <c>marketing_carrier_type</c> / <c>mpa_spec</c> /
    /// <c>dca_spec</c> / <c>bid_scene</c> / <c>smart_bid_type</c> 等），且只读字段（<c>system_status</c> /
    /// <c>created_time</c> / <c>targeting_translation</c>）也不在表内 —— 这是「官方不开放」而非「实现漏了」，
    /// 为「对齐 add」而补字段会造出官方拒绝的请求体。
    /// </para>
    /// </remarks>
    [Fact]
    public void AdgroupsFieldLevels_ShouldMatchVerifiedDocumentHierarchy()
    {
        var infoNames = OfficialJsonNames(typeof(AdsAdgroupInfo));

        infoNames.Should().Contain(new[]
            {
                "targeting", "targeting_translation", "poi_list", "marketing_asset_outer_spec",
                "scene_spec", "user_action_sets", "deep_conversion_spec", "industry_value_explore",
                "mpa_spec", "dca_spec", "marketing_goal", "configured_status", "daily_budget",
            },
            "adgroups/get 应答的顶层字段集（层级即官方 DOM 层级，平面读法会把前三支误判为子字段）");

        // ① 三支「顶层但紧邻 struct」的字段不得被子结构吞掉。
        var targetingNames = OfficialJsonNames(typeof(AdsAdgroupTargeting));
        targetingNames.Should().NotContain("targeting_translation",
            "官方 targeting_translation 与 targeting 平级、由官方按定向内容生成（只读），挂在定向结构里绑不上");
        targetingNames.Should().NotContain("poi_list", "poi_list 属顶层字段，与定向结构无关");

        var assetNames = OfficialJsonNames(typeof(AdsMarketingAssetOuterSpec));
        assetNames.Should().NotContain("poi_list",
            "官方 poi_list 与 marketing_asset_outer_spec 平级（平面读法最易把两者当成父子）");

        // ② 三支确在 industry_value_explore 之内（反向：顶层不得再出现一份，出现即双源）。
        OfficialJsonNames(typeof(AdsIndustryValueExplore)).Should()
            .Contain(new[] { "iaa_smart_hosting", "high_volume_exploration", "prospect_retargeting" },
                "2026-10-10 按 DOM 层级核验：这三支确实是 industry_value_explore 的子字段");
        infoNames.Should().NotContain("prospect_retargeting",
            "prospect_retargeting 只在 industry_value_explore 内出现一份；顶层再出现即是层级双源");

        // ③ 报文数组名取官方请求参数表，不跟官方使用说明的笔误。
        OfficialJsonNames(typeof(AdsAdgroupUpdateDatetimeRequest)).Should()
            .Equal(new[] { "account_id", "update_datetime_spec" },
                "官方使用说明处误写 update_date_spec，请求参数表才是报文权威");
        AdsJsonPropertyNames().Should().NotContain("update_date_spec",
            "全仓不得出现官方笔误形态（一旦出现即说明有人跟着误写走）");

        // ④ update 面是官方开放面的镜像，不是 add 面的子集对齐。
        var updateNames = OfficialJsonNames(typeof(AdsAdgroupUpdateRequest));
        updateNames.Should().Contain(new[] { "account_id", "adgroup_id" },
            "官方 update 页顶层仅此两支标必填");

        var notOpenOnUpdate = new[]
        {
            "marketing_goal", "marketing_carrier_type", "marketing_sub_goal", "site_set",
            "automatic_site_enabled", "mpa_spec", "dca_spec", "marketing_asset_outer_spec",
            "marketing_asset_id", "bid_scene", "smart_bid_type", "targeting_translation",
        };

        updateNames.Should().OnlyContain(n => !notOpenOnUpdate.Contains(n),
            "以下字段官方 adgroups/update 页未开放，不得为「与 add 对齐」而补上：" +
            string.Join(", ", notOpenOnUpdate.Intersect(updateNames)));

        // 同一棵子树在 add / update / get 三页共用同一批类型：字段级共用可防「三处复制各自漂移」。
        var addNames = OfficialJsonNames(typeof(AdsAdgroupAddRequest));
        addNames.Should().Contain(new[] { "account_id", "adgroup_name", "marketing_goal", "marketing_carrier_type", "begin_date", "end_date", "time_series" },
            "官方 add 页的顶层必填集（SDK 不本地拦截，但字段必须都在）");
        addNames.Should().NotContain("adgroup_id",
            "add 是新建资源、由官方分配 id，官方请求表不含 adgroup_id（含即说明请求体复用了应答类型）");

        // 共用子树必须<b>同一 CLR 类型</b>：get / add / update 三页的 targeting 等结构形状相同，
        // 各建一份「同名不同类」的结构 = 三处漂移（官方改子字段时只会改到一处）。
        foreach (var shared in new[]
                 {
                     "targeting", "scene_spec", "user_action_sets", "deep_conversion_spec",
                     "industry_value_explore", "poi_list", "configured_status",
                 })
        {
            var onAdd = JsonPropertyClrType(typeof(AdsAdgroupAddRequest), shared);
            var onUpdate = JsonPropertyClrType(typeof(AdsAdgroupUpdateRequest), shared);
            var onInfo = JsonPropertyClrType(typeof(AdsAdgroupInfo), shared);

            onAdd.Should().NotBeNull($"{shared} 必须声明在 AdsAdgroupAddRequest 上（缺即本域漏建模）");
            onAdd.Should().Be(onUpdate, $"{shared} 在 add 与 update 两页必须是同一类型（分别建模即双源）");
            onAdd.Should().Be(onInfo, $"{shared} 在请求与应答两侧必须是同一类型（应答侧另建即三源）");
        }
    }

    /// <summary>
    /// ADS-B2（<b>报表域路由表</b>）：<c>daily_reports</c> / <c>hourly_reports</c> / <c>async_reports</c>
    /// 三支资源族共 4 端点的<b>方法 × 路由 × HTTP 动词</b>逐条等于官方原文。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>本族的动词形态与仓内一条经验相反</b>（2026-10-10 逐页核验，留档 §7.8 已列为族级结论）：
    /// 报表族<b>全部</b> <c>*/get</c> 端点官方即 GET，只有 <c>async_reports/add</c> 是 POST +
    /// <c>application/json</c>。「多数查询类接口官方即 POST」<b>不能</b>套到本族 —— 给 GET 页补 POST
    /// 会直接打不开（官方页面自己写明请求方法），所以唯一处置是照录。
    /// </para>
    /// <para>
    /// <b>一个注册组跨三支资源族是有意为之</b>（理由见 <c>AdsModule.Reports</c>），因此本条按<b>路由前缀</b>
    /// 再断言一次族划分：<c>daily_reports</c> 一支、<c>hourly_reports</c> 一支、<c>async_reports</c> 两支。
    /// 合并模块不等于合并契约面 —— 族级差异（level 三支集合、分页上限、<c>organization_id</c> 有无）
    /// 全部留在逐方法参数面，由 <see cref="ReportQueryParameters_ShouldMatchPerPageOfficialTables"/> 逐页锁定。
    /// </para>
    /// <para>
    /// <c>async_report_files/get</c> <b>不在本表</b>：该页请求地址逐字是<b>另一台主机</b>
    /// <c>https://dl.e.qq.com/v3.0/async_report_files/get</c>，而本线当前只有一条指向 <c>api.e.qq.com</c>
    /// 的业务客户端 ⇒ 未落地。本条把「未落地」钉成<b>可见的</b>计数（恰 4 支而非 5 支），
    /// 而不是留一条空断言假装覆盖。
    /// </para>
    /// </remarks>
    [Fact]
    public void ReportsEndpoints_ShouldMatchOfficialRouteTable()
    {
        ReportsRoutes.Should().HaveCount(4,
            "本域落地 daily/hourly/async_reports(add+get) 四支；async_report_files/get 因基址不同而未落地");
        ReportsRoutes.Select(static r => r.Route).Distinct().Should().HaveCount(4, "路由互不重复");
        ReportsRoutes.Select(static r => r.Interface).Distinct().Should().HaveCount(1,
            "本表只描述 IWechatAdsReportService 一个注册组（混入它域路由会让端点计数失去意义）");

        foreach (var (family, expected) in new[]
                 {
                     ("/v3.0/daily_reports/", 1),
                     ("/v3.0/hourly_reports/", 1),
                     ("/v3.0/async_reports/", 2),
                 })
        {
            ReportsRoutes.Count(r => r.Route.StartsWith(family, StringComparison.Ordinal))
                .Should().Be(expected, $"{family} 族的端点数即官方资源族的动作数（合并模块不得抹平族级划分）");
        }

        typeof(IWechatAdsReportService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().HaveCount(4, "接口端点数与守卫路由表条目数必须一致（新增端点未入表即红，防「实现了但没锁」）");

        foreach (var (iface, method, httpAttribute, route) in ReportsRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");

            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr.Should().NotBeNull($"{iface.Name}.{method} 必须声明与官方一致的 HTTP 动词");
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约逐字符一致");
        }

        // 族级动词事实：本域三支查询官方即 GET，只有 add 是 POST（照录，不得「对齐」仓内另一条经验）。
        ReportsRoutes.Count(static r => r.HttpAttribute == typeof(GetAttribute)).Should().Be(3,
            "daily_reports/get、hourly_reports/get、async_reports/get 三支官方请求方法行均写 GET");
        ReportsRoutes.Single(static r => r.HttpAttribute == typeof(PostAttribute)).Route
            .Should().Be("/v3.0/async_reports/add", "本域唯一的 POST 是创建异步报表任务");
    }

    /// <summary>
    /// ADS-B2 / ADS-B5（<b>报表域逐页参数面</b>）：三支查询页的 Query 参数名集合<b>逐页不同</b>，
    /// 本域四页<b>都没有</b>「特定请求参数」表（不带 <c>user_token</c>）。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>三处「看着不一致其实官方就不一致」的点，逐条钉成参数名集合的相等断言</b>：
    /// ① <c>organization_id</c> 只在 daily 与 async_reports/get 两页有，<c>hourly</c> 页<b>没有</b>
    /// ⇒ 「三支查询对齐成同一签名」会发送官方未定义的参数；
    /// ② 必填面：daily 的 <c>account_id</c> 未加星（但描述实际必填）、hourly 加了星、async_reports/get
    /// 标选填 ⇒ 三者的 CLR 可空性<b>不同</b>（见 <see cref="ReportQueryParameterNullability_ShouldFollowPerPageStar"/>）；
    /// ③ 五支复合参数（<c>date_range</c> / <c>fields</c> / <c>group_by</c> / <c>filtering</c> / <c>order_by</c>）
    /// 只能是 <see cref="string"/>：官方 curl 逐字把 <c>date_range</c> 写成<b>单个</b>键值对、值为 JSON
    /// 对象字面量，组件对数组走「重复同名参数」、对复杂类型走「逐属性展平」，两种都不是官方线格式。
    /// </para>
    /// <para>
    /// <b><c>user_token</c> 缺席本域是官方事实</b>：四页均无「特定请求参数」表 ⇒ 一处都不带。
    /// 若「与 adgroups 的 POST 面保持一致」而给 <c>async_reports/add</c> 加上，等于把凭据塞进官方不读的
    /// Query 参数并多一处外泄面（ADS-B5 的登记面管不住「不该带的参数」，故由本条点名）。
    /// </para>
    /// </remarks>
    [Fact]
    public void ReportQueryParameters_ShouldMatchPerPageOfficialTables()
    {
        var methods = typeof(IWechatAdsReportService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .ToArray();

        var daily = methods.Single(static m => m.Name == nameof(IWechatAdsReportService.GetDailyAsync));
        var hourly = methods.Single(static m => m.Name == nameof(IWechatAdsReportService.GetHourlyAsync));
        var asyncAdd = methods.Single(static m => m.Name == nameof(IWechatAdsReportService.AddAsyncReportAsync));
        var asyncGet = methods.Single(static m => m.Name == nameof(IWechatAdsReportService.GetAsyncReportAsync));

        QueryParameterNames(daily).Should().Equal(new[]
        {
            "account_id", "level", "date_range", "fields", "group_by", "filtering", "order_by",
            "time_line", "page", "page_size", "organization_id",
        }, "官方 daily_reports/get 请求参数表的拼写与顺序（必填在前、其余照官方表格次序，与 advertiser/get 同口径）");

        QueryParameterNames(hourly).Should().Equal(new[]
        {
            "account_id", "level", "date_range", "fields", "group_by", "filtering", "order_by",
            "time_line", "page", "page_size",
        }, "官方 hourly_reports/get 参数表<b>没有 organization_id</b>（daily 页有）⇒ 补它就是发送未定义参数");

        QueryParameterNames(asyncGet).Should().Equal(new[]
        {
            "account_id", "filtering", "page", "page_size", "organization_id",
        }, "async_reports/get 只有任务面参数：官方本页无 level / granularity / date（那三支只在 add 页）");

        QueryParameterNames(asyncAdd).Should().BeEmpty(
            "async_reports/add 是 POST + application/json，全部业务参数进请求体；" +
            "本页官方既无 user_token（无「特定请求参数」表）也无 X-Request-Id（无幂等头表）");

        // 复合参数的承载形态：JSON 字面量字符串，不是集合也不是复杂类型。
        foreach (var (method, names) in new[] { (daily, new[] { "date_range", "fields", "group_by", "filtering", "order_by" }),
                                                (hourly, new[] { "date_range", "fields", "group_by", "filtering", "order_by" }) })
        {
            var declared = method.GetParameters()
                .Select(p => (Name: p.GetCustomAttribute<QueryAttribute>()?.Name, Type: p.ParameterType))
                .Where(static p => p.Name is not null)
                .ToList();

            foreach (var name in names)
            {
                TypeOf(declared, name).Should().Be(typeof(string),
                    $"官方 {name} 是 struct[] / string[] / struct，必须以 JSON 字面量字符串上送（{method.Name}）");
            }
        }

        // 本域四页均无 user_token（与 adgroups 的 POST 面形成对照，那一面对照由
        // AdgroupsQueryCredentialAndIdempotencyHeader 守卫锁定）。
        foreach (var method in methods)
        {
            QueryParameterNames(method).Should().NotContain("user_token",
                $"{method.Name} 所在页官方未列出「特定请求参数」表 ⇒ 不得带实名认证令牌");
        }
    }

    /// <summary>
    /// ADS-B2（<b>必填星号逐页不同 ⇒ CLR 可空性也不同</b>）：<c>account_id</c> 在三支查询页的形态。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 官方三页对同一支 <c>account_id</c> 给了<b>三种</b>形态（2026-10-10 逐页核验）：
    /// <c>daily_reports/get</c> <b>未加星</b>但描述「有操作权限的帐号 id，不支持代理商 id」（留档登记的矛盾项，
    /// 建模取<b>实际必填</b> ⇒ 非可空）；<c>hourly_reports/get</c> <b>加了星</b> ⇒ 非可空；
    /// <c>async_reports/get</c> 标<b>选填</b>且描述「拥有操作权限的账户 ID」⇒ 可空。
    /// </para>
    /// <para>
    /// 把三支统一成同一种可空性是<b>两种错误各占其一</b>：全非空会让 async 页的合法「不传账户」调用无法表达；
    /// 全可空则把 daily/hourly 的必填参数变成「忘了传也能发出去」，错误推迟到官方网关。
    /// </para>
    /// </remarks>
    [Fact]
    public void ReportQueryParameterNullability_ShouldFollowPerPageStar()
    {
        var reported = typeof(IWechatAdsReportService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Select(m => (Method: m.Name,
                          AccountId: m.GetParameters()
                              .FirstOrDefault(p => p.GetCustomAttribute<QueryAttribute>()?.Name == "account_id")
                              ?.ParameterType))
            .Where(static x => x.AccountId is not null)
            .ToDictionary(static x => x.Method, static x => x.AccountId!, StringComparer.Ordinal);

        reported[nameof(IWechatAdsReportService.GetDailyAsync)].Should().Be(typeof(long),
            "daily 页虽未加星，但描述实际必填 ⇒ 取非可空（星号是本页的文档缺陷，不是官方允许缺省）");
        reported[nameof(IWechatAdsReportService.GetHourlyAsync)].Should().Be(typeof(long),
            "hourly 页官方加了必填星 ⇒ 非可空");
        reported[nameof(IWechatAdsReportService.GetAsyncReportAsync)].Should().Be(typeof(long?),
            "async_reports/get 页官方标选填 ⇒ 可空，不得因「与另两支一致」而改成必填");

        // 代理商 id 的支持面也是逐页事实，只允许写在文档里、不允许被代码「统一」：
        // daily / hourly 两支同步页原文明确「不支持代理商 id」（写在接口 XML），
        // async_reports/add 页写「包括代理商和账户 id」（该页 account_id 是请求体字段 ⇒ 写在 DTO 上）。
        var interfaceText = File.ReadAllText(AdsReportInterfaceFile);
        CountOf(interfaceText, "不支持代理商 id").Should().Be(2,
            "只有 daily 与 hourly 两页声明「不支持代理商 id」；第三处命中说明有人把它套到了 async 面上");

        var asyncAddDtoText = File.ReadAllText(AdsAsyncReportAddFile);
        CountOf(asyncAddDtoText, "包括代理商和账户 id").Should().Be(2,
            "async_reports/add 页的 account_id 支持代理商 id，与同步两页相反（留档矛盾登记）：" +
            "类 remarks 记矛盾、属性 summary 记取值口径，两处缺一即文档被删");
    }

    /// <summary>
    /// ADS-B2（<b><c>level</c> 三支集合互不相同、不得合并</b>）：从接口 XML 的逐页原文抽取
    /// <c>REPORT_LEVEL_*</c> 集合并与官方核验值逐条相等。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>这是本域最容易被「顺手收敛」破坏的一处</b>：官方三页各给一支集合
    /// （daily 17 / hourly 8 / async_reports/add 21），且差异是<b>双向</b>的 —— async 页既比 daily
    /// 多出人口属性等七支、又少了 <c>VIDEO_HIGHLIGHT</c> / <c>WECHAT_SHOP_PRODUCT</c> / <c>PLAYLET</c> 三支。
    /// 「三支合并成一支公共枚举」看起来是消重，实际造出的枚举值里有三分之一在任意一页上都不被接受；
    /// 而「按 daily 集合调 hourly」是官方直接拒绝的请求。
    /// </para>
    /// <para>
    /// <b>为什么从 XML 抽而不是从类型抽</b>：本域刻意<b>不建</b>本地枚举（下方同时断言 DataModels 里
    /// <c>enum</c> 类型数为零），所以集合的唯一存在形态就是各端点的参数文档。
    /// 抽取按逐页锚点定位，锚点缺失即红 —— 这是「文档被改写」与「文档被删除」的唯一可机械检出形态。
    /// </para>
    /// </remarks>
    [Fact]
    public void ReportLevelSets_ShouldStayThreeDistinctPerPageCollections()
    {
        var text = File.ReadAllText(AdsReportInterfaceFile);

        var daily = ExtractReportLevels(text, "daily_reports/get）官方可选值");
        var hourly = ExtractReportLevels(text, "hourly_reports/get）官方可选值");
        var asyncAdd = ExtractReportLevels(text, "async_reports/add）官方可选值");

        daily.Should().Equal(DailyReportLevels.OrderBy(static n => n, StringComparer.Ordinal),
            "官方 daily_reports/get 的 level 集合（17 支，逐字核验）");
        hourly.Should().Equal(HourlyReportLevels.OrderBy(static n => n, StringComparer.Ordinal),
            "官方 hourly_reports/get 的 level 集合（8 支，是 daily 的真子集）");
        asyncAdd.Should().Equal(AsyncReportAddLevels.OrderBy(static n => n, StringComparer.Ordinal),
            "官方 async_reports/add 的 level 集合（21 支：daily 增七支、删三支）");

        daily.Should().HaveCount(17);
        hourly.Should().HaveCount(8);
        asyncAdd.Should().HaveCount(21);

        // 三支两两不等，且差异方向被钉住（合并 = 红；只往一个方向收敛 = 也红）。
        hourly.Should().BeSubsetOf(daily, "hourly 层级集合是 daily 的子集（官方页面事实）");
        daily.Should().NotBeEquivalentTo(asyncAdd, "async 页与 daily 页的集合互不相同，不得收敛成公共枚举");
        hourly.Should().NotBeEquivalentTo(asyncAdd);
        daily.Intersect(AsyncReportAddLevels).Should().HaveCount(14,
            "daily ∩ async = 17 − 3 = 14 支（async 独有的 7 支不得出现在 daily 面上）");
        daily.Except(AsyncReportAddLevels).Select(static n => n).Should().Equal(new[]
        {
            "REPORT_LEVEL_PLAYLET", "REPORT_LEVEL_VIDEO_HIGHLIGHT", "REPORT_LEVEL_WECHAT_SHOP_PRODUCT",
        }, "daily 有而 async 无的恰是这三支");

        // 本域不建本地枚举 ⇒ DataModels 里 enum 类型数必须为 0（有即「公共枚举必然失真」的那条老路）。
        AdsDataModelTypes().Where(static t => t.IsEnum).Select(static t => t.FullName)
            .Should().BeEmpty(
            "官方取值集合逐页不同且会随页面演进，做成本地枚举必然与某一页脱节 ⇒ 一律以 string 承载、可选值写进 XML");
    }

    /// <summary>
    /// ADS-B2（<b>报表域载荷的单源与形状</b>）：daily / hourly 共用一支 <c>data</c>，
    /// 行取<b>透传</b>形态，<c>page_info</c> 复用公共类型，异步 <c>result</c> 层的 <c>code</c> 可空。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>行为什么不能是 DTO</b>：官方把 <c>data.list[]</c> 的元素定义为「随请求侧 <c>fields</c> 而变」，
    /// 而指标面（hourly 示例行约 150 支）只有<b>平面级</b>证据。把示例行的六支字段固化成类型，
    /// 等于把「示例」当成「契约」：调用方请求别的指标时数据静默丢失，且<b>不会有任何报错</b>。
    /// </para>
    /// <para>
    /// <b>异步 <c>result.code</c> 是内层第二个判定面</b>：外层信封 <c>code == 0</c> 只表示「查询任务这个请求成功」，
    /// 任务本身成没成看 <c>result.code</c>；与批量族逐条 <c>code</c> 同属「判错必须多层做」的一类
    /// （见 <see cref="BatchResultItems_ShouldCarryTheirOwnCode"/>）。可空性必须保留 ——
    /// 写成非空会把「任务未完成、官方整字段缺省」读成 <c>0</c>（= 成功），是最危险的一类假绿。
    /// </para>
    /// </remarks>
    [Fact]
    public void ReportPayloads_ShouldShareVerifiedShapesAndKeepInnerCodeNullable()
    {
        foreach (var responseType in new[] { typeof(AdsDailyReportResponse), typeof(AdsHourlyReportResponse) })
        {
            responseType.BaseType!.GetGenericTypeDefinition().Should().Be(typeof(AdsResponse<>),
                "闭合信封必须直接继承公共基底（自建信封会绕过 ThrowIfFailed 的单层判定）");
            responseType.BaseType!.GetGenericArguments().Single().Should().Be(typeof(AdsReportListData),
                "官方两页应答字段表同形 ⇒ 只允许一支共用载荷");
        }

        var itemType = typeof(AdsReportListData)
            .GetProperty("List", BindingFlags.Public | BindingFlags.Instance)!
            .PropertyType.GetGenericArguments().Single();

        itemType.Should().Be(typeof(Dictionary<string, JsonElement>),
            "行必须按透传建模：键集由请求侧 fields 决定，固化 DTO 会让未请求字段静默丢数据");
        itemType.GetGenericArguments().Select(static t => t.FullName)
            .Should().Equal(new[] { "System.String", "System.Text.Json.JsonElement" },
                "值保留原始 JSON 形态（金额整数分 / 比率浮点 / 维度字符串），SDK 不做单位换算与类型猜测");

        // page_info 复用公共类型；async 侧的行则是固定结构（两族不同形，不收敛）。
        JsonPropertyClrType(typeof(AdsReportListData), "page_info").Should().Be(typeof(AdsPageInfo));
        JsonPropertyClrType(typeof(AdsAsyncReportListData), "page_info").Should().Be(typeof(AdsPageInfo));
        JsonPropertyClrType(typeof(AdsAsyncReportListData), "list")!.GetGenericArguments().Single()
            .Should().Be(typeof(AdsAsyncReportTaskInfo),
            "异步任务行是固定结构（task_id / status / result…），与同步报表的透传行不同形 ⇒ 两支各建各的");

        JsonPropertyClrType(typeof(AdsAsyncReportTaskInfo), "result").Should().Be(typeof(AdsAsyncTaskResult));
        typeof(AdsAsyncTaskResult)
            .GetProperty("Code", BindingFlags.Public | BindingFlags.Instance)!
            .PropertyType.Should().Be(typeof(int?),
            "result.code 可缺省（任务未完成）⇒ 必须可空；非空会把「缺省」读成 0 = 成功");

        // 官方本页的 result 只有 code / message / data 三键，没有 message_cn（与外层信封不同）。
        OfficialJsonNames(typeof(AdsAsyncTaskResult)).Should().Equal(new[] { "code", "data", "message" },
            "异步任务 result 层的官方键集；多出 message_cn 即越出该页契约");

        // 字段清单那支：同步两页叫 fields，异步 add 页叫 report_fields ⇒ 报文根上不得出现 fields 键。
        OfficialJsonNames(typeof(AdsAsyncReportAddRequest)).Should().Contain("report_fields",
            "async_reports/add 页的字段清单键名（与同步页的 fields 不同名）");
        OfficialJsonNames(typeof(AdsAsyncReportAddRequest)).Should().NotContain("fields",
            "把 report_fields 写成 fields 会被官方静默忽略，任务建出来但没有报表字段");
        OfficialJsonNames(typeof(AdsAsyncReportAddRequest)).Should().NotContain("date_range",
            "本页官方只有单支 date（配 granularity），不存在 date_range 结构");

        // 官方异步链路上没有任何文件名 / 格式字段 ⇒ 本域不得凭空补出（下载只吃 task_id + file_id）。
        OfficialJsonNames(typeof(AdsAsyncTaskFileInfo)).Should().Equal(new[] { "file_id", "md5" },
            "整条异步链路唯一声明的完整性字段是 md5；没有 file_name / format / extension");
    }

    /// <summary>
    /// ADS-B2（<b>OAuth 两支 + 版本前缀不对称</b>）：路由与全局参数名照官方原文。
    /// </summary>
    /// <remarks>
    /// 官方 <c>oauth/token</c> / <c>oauth/refresh_token</c> <b>均为 GET</b> 且路径<b>不带</b> <c>/v3.0</c>
    /// （官方两页「请求地址」原文即为 <c>https://api.e.qq.com/oauth/token</c>），而业务端点<b>必带</b>
    /// <c>/v3.0</c>。这支不对称是本线最容易「顺手统一」成灾的地方 —— 一旦有人把 OAuth 也加上版本号，
    /// 换码会直接 404，而 404 在 <c>[AllowAnyStatusCode]</c> 下还要再过一层判错才暴露。
    /// </remarks>
    [Fact]
    public void OAuthRoutes_ShouldMatchOfficialPathsAndGlobalParameterNames()
    {
        AdsOAuthRoutes.Token.Should().Be("/oauth/token", "官方 oauth/token 请求地址原文（无 /v3.0 前缀）");
        AdsOAuthRoutes.RefreshToken.Should().Be("/oauth/refresh_token", "官方 oauth/refresh_token 原文（无 /v3.0 前缀）");
        AdsOAuthRoutes.GrantTypeAuthorizationCode.Should().Be("authorization_code");
        AdsOAuthRoutes.GrantTypeRefreshToken.Should().Be("refresh_token");
        AdsOAuthRoutes.AccessTokenParameter.Should().Be("access_token");
        AdsOAuthRoutes.TimestampParameter.Should().Be("timestamp");
        AdsOAuthRoutes.NonceParameter.Should().Be("nonce");

        // OAuth 两支是 GET + Query 手写拼装，参数名<b>不</b>经 DTO 声明 ⇒ 从源码字面量面钉官方原文
        // （AdsDataModelJsonNames 那条只覆盖 [JsonPropertyName]，覆盖不到这条手写面）。
        AdsOAuthRequestParameterNames().Should()
            .Equal(new[] { "authorization_code", "client_id", "client_secret", "grant_type", "redirect_uri", "refresh_token" },
                "官方 oauth/token 与 oauth/refresh_token 两页请求参数表的拼写与集合（新增/改名须同批改本表并核对官方页）");

        // 版本前缀对<b>全部</b>业务路由生效（不只首批域）：新增域忘带 /v3.0 时打的是无版本路径，
        // 官方网关的返回形态与「资源不存在」同类，最难归因。
        // 本循环只遍历守卫表；守卫表与接口反射面的<b>相等性</b>由
        // AdsRoutes_ShouldEqualGuardTablesAndNeverReviveRemovedResources 锁定，故不存在「新域绕过本循环」的窗口。
        foreach (var route in AdsGuardedRoutes().Select(static r => r.Route))
        {
            route.Should().StartWith("/v3.0/", "业务端点路由必须带官方版本前缀（基址只到主机名）");
        }
    }

    /// <summary>
    /// ADS-B2（<b>请求参数名与复合参数承载形态</b>）：<c>advertiser/get</c> 的 Query 参数表。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 参数名照官方原文（<c>account_id</c> / <c>pagination_mode</c> / <c>page_size</c> / <c>agency_id</c>
    /// 不得驼峰化）。<c>fields</c> 与 <c>filtering</c> 官方是 <c>string[]</c> / <c>struct[]</c>，
    /// 但<b>声明成数组或复杂类型都拿不出官方线格式</b>（组件对数组走「重复同名参数」、对复杂类型走
    /// 「逐属性展平」）⇒ 只能是 <see cref="string"/>，构造入口唯一 <see cref="AdsQueryJson"/>。
    /// </para>
    /// <para>同批锁定 ADS-B1 的正面：全局参数三件套不得出现在任何端点签名里（由传输层成组注入）。</para>
    /// </remarks>
    [Fact]
    public void AdvertiserGetQueryParameters_ShouldMatchOfficialNamesAndCarryCompositeAsJsonString()
    {
        var getAsync = typeof(IWechatAdsAdvertiserService)
            .GetMethod(nameof(IWechatAdsAdvertiserService.GetAsync),
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
        getAsync.Should().NotBeNull();

        var declared = getAsync!.GetParameters()
            .Select(p => (Name: p.GetCustomAttribute<QueryAttribute>()?.Name, Type: p.ParameterType))
            .Where(p => p.Name is not null)
            .ToList();

        declared.Select(static p => p.Name).Should()
            .Equal(new[] { "account_id", "fields", "pagination_mode", "page_size", "agency_id", "filtering", "page", "cursor" },
                "官方 advertiser/get 请求参数表的拼写与顺序（新增/改名须同批改本表并核对官方页）");

        TypeOf(declared, "fields").Should().Be(typeof(string),
            "官方 string[] 必须以 JSON 数组字符串上送：数组形态会被组件展开成重复同名参数");
        TypeOf(declared, "filtering").Should().Be(typeof(string),
            "官方 struct[] 必须以 JSON 数组字符串上送：复杂类型会被组件逐属性展平成 page[0].field=… 形态");

        var globalNames = new[]
        {
            AdsOAuthRoutes.AccessTokenParameter, AdsOAuthRoutes.TimestampParameter, AdsOAuthRoutes.NonceParameter,
        };

        var leaked = AdsAllQueryParamNames().Where(n => ContainsIgnoreCase(globalNames, n)).ToArray();
        leaked.Should().BeEmpty(
            $"全局参数 {string.Join(" / ", globalNames)} 由 AdsAuthorizationHandler 在发送前成组注入，" +
            "出现在端点签名里即允许调用方各传一份，破坏「一次请求一组凭据」");
    }

    /// <summary>
    /// ADS-B2（<b>字段名不驼峰化</b>）：广告线 DTO 的<b>每一个</b>公共属性都必须显式带官方 snake_case
    /// <c>[JsonPropertyName]</c>，且名字形状合法。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>为什么必须是「覆盖面」断言，而不是只校验已声明的名字</b>（生成物逐个核验，2026-10-10）：
    /// 广告线五个上下文当前<b>都</b>取 <c>PropertyNamingPolicy = SnakeCaseLower</c> ⇒ 漏标看似仍得到
    /// snake_case，但这是<b>脚手架产出</b>的事实、不是契约：策略逐上下文独立声明，重跑脚本即可变动，
    /// 而 DTO 换一个目录就换到另一个上下文。任何一侧漂移时，漏标属性的线上名<b>静默</b>改形
    /// （信封的 <c>message_cn</c> 落进 camelCase 上下文即写成 <c>messageCn</c>，官方解析不到该键）——
    /// 上送被官方静默忽略、应答属性恒为 <c>null</c>，两边都不报错。
    /// 因此「每个公共属性必须显式声明」是唯一<b>不依赖上下文策略</b>的形态，
    /// 而「只查显式声明的名字形状」是一种<b>假绿</b>。
    /// </para>
    /// <para>
    /// 本条因此做三面判定：① <b>覆盖面</b> —— 逐类型逐公共属性要求「显式 <c>[JsonPropertyName]</c>
    /// <b>或</b> 显式 <c>[JsonIgnore]</c>」（后者是判错门面这类便利属性的唯一合法出口，例如
    /// <see cref="AdsResponse.ErrorCode"/> / <see cref="AdsResponse.IsSuccess"/>；两者都不写即默认按
    /// 所在上下文的策略输出，等于凭空多出官方未知字段）。生成上下文（<c>*JsonContext</c>）不参与：
    /// 生成物随脚本重跑而变，其内部命名不属官方契约面；② <b>形状</b> —— 显式名必须匹配
    /// <c>^[a-z][a-z0-9_]*$</c>（驼峰 / 大写 / 空格都会被官方静默忽略）；
    /// ③ <b>关键名</b> —— 逐条锁定最容易被「顺手改规范」的名字（<c>message_cn</c> / <c>cursor_page_info</c> /
    /// <c>fail_id_list</c> / <c>update_daily_budget_spec</c> / <c>next_cursor</c> 等，
    /// 后几支不是常规命名，凭直觉改必错）。
    /// </para>
    /// <para>
    /// <b>另一处同源陷阱（本条抓不到、由 ADS-B6 与 DTO remarks 承担）</b>：上下文还设了
    /// <c>DefaultIgnoreCondition = WhenWritingNull</c> ⇒ 「显式 null」与「字段不写」在线上不可区分，
    /// 需要清空官方字段的调用方不能靠传 <c>null</c> 表达。
    /// </para>
    /// </remarks>
    [Fact]
    public void AdsDataModelJsonNames_ShouldStayOfficialSnakeCase()
    {
        var declared = AdsJsonPropertyNames();

        declared.Should().NotBeEmpty("广告线 DTO 应全部显式声明官方字段名");

        var wireTypes = AdsWireDtoTypes();
        wireTypes.Should().NotBeEmpty();

        var undocumented = wireTypes
            .SelectMany(t => t
                .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Where(p => p.GetCustomAttribute<JsonPropertyNameAttribute>() is null
                            && p.GetCustomAttribute<JsonIgnoreAttribute>() is null)
                .Select(p => t.Name + "." + p.Name))
            .OrderBy(static n => n, StringComparer.Ordinal)
            .ToArray();

        undocumented.Should().BeEmpty(
            "以下公共属性既无 [JsonPropertyName] 也无 [JsonIgnore]：" + string.Join(", ", undocumented) +
            " ⇒ 输出名由所在上下文的 PropertyNamingPolicy 决定，而策略是脚手架产出、非契约（重跑脚本或换目录即可改形），"
            + "于是「是否合官方契约」变成「文件放在哪个目录」的偶然事实；判错门面类属性（如信封的 "
            + "ErrorCode / IsSuccess）必须显式 [JsonIgnore]，否则会成为官方未知字段");

        var malformed = declared
            .Where(static n => !OfficialNamePattern.IsMatch(n))
            .OrderBy(static n => n, StringComparer.Ordinal)
            .ToArray();

        malformed.Should().BeEmpty(
            "以下官方字段名被写成非 snake_case（驼峰 / 大写 / 空格都会让官方静默忽略该字段）：" +
            string.Join(", ", malformed));

        declared.Should().Contain(new[]
            {
                "code", "message", "message_cn", "data",
                "page_info", "cursor_page_info", "total_number", "total_page", "has_more", "cursor",
                "next_cursor", "previous_cursor",
                "list", "fail_id_list", "update_daily_budget_spec", "use_min_daily_budget",
                "update_configured_status_spec", "update_bid_amount_spec", "update_datetime_spec",
                "account_id", "agency_account_id", "daily_budget", "system_status", "is_adx",
                "adgroup_id", "adgroup_name", "configured_status", "bid_amount", "begin_date", "end_date", "time_series",
                "marketing_goal", "marketing_carrier_type", "marketing_asset_outer_spec",
                "promoted_asset_type", "deep_optimization_type", "cost_guarantee_status", "data_model_version",
                "auto_derived_creative_method_type_list", "targeting_translation", "poi_list",
                "scene_spec", "wechat_scene", "user_action_sets", "deep_conversion_spec", "mpa_spec", "dca_spec",
                "prospect_retargeting", "industry_value_explore",
                "individual_qualification", "identification_front_image_id", "icp_image_id",
                "operator_id", "wechat_account_id", "authorizer_info", "scope_list", "account_role_type",
                "access_token", "refresh_token", "access_token_expires_in", "refresh_token_expires_in",
                // 报表族（2026-10-10 逐页核验）：report_fields 与同步页的 fields 不同名、
                // date/granularity/time_line 三支是本页特有形态，task_id 链路上没有任何文件名字段。
                "report_fields", "granularity", "time_line", "level", "date", "organization_id",
                "task_id", "task_name", "status", "created_time", "result",
                "file_info_list", "file_id", "md5",
                "sort_field", "sort_type", "start_date", "end_date",
            },
            "这些名字直接照官方应答/请求表原文，是各页核验的落地形态，任何一个被规范化都会造成静默字段丢失");
    }

    /// <summary>
    /// ADS-B2（<b>字段名单源</b>）：<see cref="AdsQueryJson"/> 写出的<b>全部</b>键名必须与三支复合
    /// DTO 的 <c>[JsonPropertyName]</c> 集合<b>恰等</b> —— <see cref="AdsFiltering"/> 的
    /// <c>field</c>/<c>operator</c>/<c>values</c>、<see cref="AdsDateRange"/> 的
    /// <c>start_date</c>/<c>end_date</c>、<see cref="AdsOrderBy"/> 的 <c>sort_field</c>/<c>sort_type</c>。
    /// </summary>
    /// <remarks>
    /// <para>
    /// 每支复合参数都有<b>两条独立产出路径</b>：DTO 声明（供应答侧与请求体序列化）与编码器手写
    /// <c>WriteString</c>（供 Query 上送）。两处各写一遍就是双源，改名时最容易只改其中一处而
    /// <b>两边都还能编译</b>，缺陷要到官方网关报 <c>code != 0</c> 才暴露。
    /// </para>
    /// <para>
    /// <b>双向都断言</b>：DTO 的键在编码器里缺 ⇒ 编码器写不出该键（漏建模）；编码器里有 DTO 没有的键 ⇒
    /// 有人手打了一个不在契约上的名字。故末条断言取「集合<b>恰等</b>」而非「包含」。
    /// </para>
    /// </remarks>
    [Fact]
    public void AdsQueryJsonKeys_ShouldMatchCompositeDtoContracts()
    {
        foreach (var (dto, expected) in new[]
                 {
                     // 期望集按 Ordinal 升序书写 —— OfficialJsonNames 的返回形态即排序后集合。
                     (typeof(AdsFiltering), new[] { "field", "operator", "values" }),
                     (typeof(AdsDateRange), new[] { "end_date", "start_date" }),
                     (typeof(AdsOrderBy), new[] { "sort_field", "sort_type" }),
                 })
        {
            OfficialJsonNames(dto).Should().Equal(expected,
                $"{dto.Name} 的官方键名集合（拼写即契约；键序对官方解析无影响，故只锁集合）");
        }

        var encoderKeys = Regex
            .Matches(StripComments(File.ReadAllText(AdsEncoderFile)), @"""([a-z][a-z0-9_]*)""")
            .Cast<System.Text.RegularExpressions.Match>()
            .Select(static m => m.Groups[1].Value)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(static n => n, StringComparer.Ordinal)
            .ToArray();

        var dtoKeys = new[] { typeof(AdsFiltering), typeof(AdsDateRange), typeof(AdsOrderBy) }
            .SelectMany(t => OfficialJsonNames(t))
            .Distinct(StringComparer.Ordinal)
            .OrderBy(static n => n, StringComparer.Ordinal)
            .ToArray();

        encoderKeys.Should().Equal(dtoKeys,
            "编码器写出的键集必须与三支复合 DTO 的键集恰等：编码器多出的键不在官方契约上，" +
            $"缺的键说明 DTO 与手写线格式已分叉（DTO：{string.Join(", ", dtoKeys)}；编码器：{string.Join(", ", encoderKeys)}）");
    }

    /// <summary>
    /// ADS-B2（<b>双层失败语义</b>）：批量族应答的逐条结果元素必须自带信封四字段中的 <c>code</c>，
    /// 且同形端点<b>共用</b>一支载荷。
    /// </summary>
    /// <remarks>
    /// 官方 <c>advertiser/update_daily_budget</c> 的 <c>data.list[]</c> 每条<b>再带一次</b>
    /// <c>code</c> / <c>message</c> / <c>message_cn</c>：外层 <c>code == 0</c> 只表示「请求被受理」，
    /// 半数失败时外层仍是 0 ⇒ 只判外层即把部分失败当全成功。本条锁死「元素侧确有独立判定面」。
    /// <para>
    /// <c>adgroups</c> 四支批量端点（<c>update_daily_budget</c> / <c>update_configured_status</c> /
    /// <c>update_bid_amount</c> / <c>update_datetime</c>）2026-10-10 逐页核验<b>应答字段表完全一致</b>
    /// （<c>{code, message, message_cn, adgroup_id}</c> + <c>fail_id_list</c>）⇒ 只允许一支
    /// <see cref="AdsAdgroupBatchData"/>；四支各建一份同形载荷就是四源，官方改字段时只会改到一处。
    /// </para>
    /// </remarks>
    [Fact]
    public void BatchResultItems_ShouldCarryTheirOwnCode()
    {
        typeof(AdsAdvertiserDailyBudgetResultItem).BaseType.Should().Be(typeof(AdsBatchResultItem),
            "逐条结果的三字段信封必须复用公共基底（自建一遍就会与外层信封漂移）");

        var itemNames = typeof(AdsAdvertiserDailyBudgetResultItem)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .Where(static n => n is not null)
            .Cast<string>()
            .ToArray();

        itemNames.Should().Contain(new[] { "account_id", "daily_budget" },
            "逐条结果须回显主键与实际生效值（官方：use_min_daily_budget 可能出现「期望下调、实际上调」）");

        typeof(AdsBatchResultItem)
            .GetProperty("Code", BindingFlags.Public | BindingFlags.Instance)!
            .PropertyType.Should().Be(typeof(int?),
                "逐条 code 可缺省 ⇒ 必须可空；写成非空会把「官方没回该条 code」误判成成功 0");

        typeof(AdsAdvertiserDailyBudgetData)
            .GetProperty("FailIdList", BindingFlags.Public | BindingFlags.Instance)!
            .GetCustomAttribute<JsonPropertyNameAttribute>()!
            .Name.Should().Be("fail_id_list", "失败 id 集合是官方原文键名，非 list 的子集，不得合并建模");

        // ---- adgroups 批量族（四支端点共用一支载荷，2026-10-10 逐页核验应答字段表完全一致）----
        typeof(AdsAdgroupBatchResultItem).BaseType.Should().Be(typeof(AdsBatchResultItem),
            "营销单元批量族与账户批量族回的是同一个三字段信封，另建一份就是双源");

        typeof(AdsAdgroupBatchResultItem)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .Should().Equal(new[] { "adgroup_id" },
                "官方 data.list[] 元素 = 公共三字段 + adgroup_id，多一个字段即越出官方契约");

        typeof(AdsAdgroupBatchData)
            .GetProperty("FailIdList", BindingFlags.Public | BindingFlags.Instance)!
            .GetCustomAttribute<JsonPropertyNameAttribute>()!
            .Name.Should().Be("fail_id_list", "汇总失败面与逐条 code 是两个判定面，不得合并建模");

        // 四支端点必须复用同一载荷类型：各建一份「形状相同、名字不同」的 data 就是四源，
        // 官方改字段时只会改到其中一处。
        var batchPayloads = new[]
        {
            typeof(AdsAdgroupUpdateDailyBudgetResponse),
            typeof(AdsAdgroupUpdateConfiguredStatusResponse),
            typeof(AdsAdgroupUpdateBidAmountResponse),
            typeof(AdsAdgroupUpdateDatetimeResponse),
        };

        foreach (var responseType in batchPayloads)
        {
            responseType.BaseType!.GetGenericTypeDefinition().Should().Be(typeof(AdsResponse<>),
                "闭合信封必须直接继承公共基底（自建信封会绕过 ThrowIfFailed 的单层判定）");
            responseType.BaseType!.GetGenericArguments().Single().Should().Be(typeof(AdsAdgroupBatchData),
                "四页应答同形 ⇒ 只允许一支共用载荷");
        }
    }

    /// <summary>
    /// 三段式（AGENTS §4）：<see cref="AdsModule"/> 成员 ⇄ 建造者 <c>Add{域}Api()</c> ⇄
    /// 接口上的 <c>[HttpClientApi(RegistryGroupName)]</c> 三者必须一一对应。
    /// </summary>
    /// <remarks>
    /// 第三段（源生成 <c>Add{组}WebApiHttpClient()</c>）无签入源文件、无法在此直接断言；
    /// 它的存在性由 <c>AddWechatAdsApi_AddAllApis_ShouldRegisterEveryLandedModule</c> 用真装配证明
    /// （组名对不上时生成方法名也不同，注册委托编译不过）。本条负责抓「枚举铺了成员但没接上注册面」这类半成品。
    /// </remarks>
    [Fact]
    public void AdsModuleRegistry_ShouldStayThreeSegmentConsistent()
    {
        var modules = Enum.GetValues(typeof(AdsModule)).Cast<AdsModule>().ToArray();

        foreach (var module in modules)
        {
            typeof(AdsServiceBuilder)
                .GetMethod("Add" + module + "Api", BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .Should().NotBeNull($"AdsModule.{module} 必须有同名链式注册方法（缺即成员铺了、装配没接）");
        }

        var groupNames = AdsBusinessInterfaces()
            .Select(static i => i.GetCustomAttribute<HttpClientApiAttribute>()!.RegistryGroupName)
            .OrderBy(static n => n, StringComparer.Ordinal)
            .ToArray();

        groupNames.Should().BeEquivalentTo(modules.Select(static m => m.ToString()),
            "注册组名集合必须与枚举成员集合相等：多出的组名无法被 AddAllApis 覆盖，缺失的枚举成员是空壳模块");

        // 可注入接口落运行时面（AGENTS §4 的形态二分；广告线暂无 IsAbstract 父接口）。
        var namespaces = AdsBusinessInterfaces().Select(static i => i.Namespace).Distinct().ToArray();
        namespaces.Should().Equal(new[] { "Mud.Wechat.Ads" },
            "广告线可注入接口一律落 Mud.Wechat.Ads（与 Work 线同一分区规则，N1 同口径）");
    }

    /// <summary>
    /// ADS-B3（<b>刷新策略与失败顺序</b>）：刷新只用 <c>oauth/refresh_token</c>，
    /// 且每条「重授权」失败路径都<b>先删存储、后抛异常</b>。
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>为什么这里做文本结构断言</b>：顺序本身有 4 条行为用例逐条锁定（见
    /// <c>AdsAuthorizationServiceTests</c> 的 <c>*_ShouldRemoveState*</c>，用存储事件序列证明
    /// <c>remove</c> 是抛出前最后一个动作），但那 4 条各只覆盖一条路径；本条覆盖<b>全部</b>抛出点 ——
    /// 新增一条失败路径而忘了删，行为用例不会红，本条会。
    /// </para>
    /// <para>
    /// <b>官方语义</b>：<c>oauth/refresh_token</c> 成功后「原 Access Token 及 Refresh Token 会失效」
    /// ⇒ 刷新失败后库里的 refresh_token 已是废纸。先抛后删会在崩溃 / 宿主吞异常下把它留下，
    /// 此后每次取令牌都撞一次无效刷新。
    /// </para>
    /// </remarks>
    [Fact]
    public void RefreshFailurePaths_ShouldDeleteStateBeforeThrowing()
    {
        var code = StripComments(File.ReadAllText(AdsAuthorizationServiceFile));

        // ① 刷新端点唯一：RefreshUnsafeAsync 只可能打 oauth/refresh_token。
        CountOf(code, "AdsOAuthRoutes.RefreshToken").Should().Be(1,
            "刷新链路的端点常量只能出现一次（多处即存在第二套刷新路径）");
        CountOf(code, "AdsOAuthRoutes.Token,").Should().Be(1,
            "oauth/token 仅供授权码换取使用（SDK 不提供 grant_type=refresh_token 入口）");

        // ② 官方 grant_type 枚举的另一支在 SDK 内无引用点 —— 有引用即意味着两套刷新语义并存。
        var grantRefreshHits = AdsSourceFiles()
            .Select(static f => new { File = Path.GetFileName(f), Code = StripComments(File.ReadAllText(f)) })
            .Where(static x => x.Code.Contains("AdsOAuthRoutes.GrantTypeRefreshToken", StringComparison.Ordinal))
            .Select(static x => x.File)
            .ToArray();
        grantRefreshHits.Should().BeEmpty(
            "SDK 刻意不提供 oauth/token?grant_type=refresh_token 入口（该页不返回 refresh_token、" +
            $"且未说明旧值是否作废）；实际命中：{string.Join(", ", grantRefreshHits)}");

        // ③ 逐抛出点：前一个存储动作必须是 RemoveAsync（抛之后不可达 ⇒ 顺序颠倒时本条红）。
        var markers = new[]
        {
            ("remove", "_store.RemoveAsync("),
            ("throw", "throw new WechatAdsReauthorizationRequiredException"),
        };

        var stream = markers
            .SelectMany(m => AllIndexesOf(code, m.Item2).Select(i => (Index: i, Kind: m.Item1)))
            .OrderBy(static x => x.Index)
            .ToList();

        var throws = stream.Where(static x => x.Kind == "throw").ToList();
        throws.Should().HaveCount(4,
            "重授权失败路径恰 4 条：无状态或无 refresh_token / refresh_token 过期 / 官方拒绝 / 应答缺令牌");

        for (var i = 0; i < stream.Count; i++)
        {
            if (stream[i].Kind != "throw")
            {
                continue;
            }

            i.Should().BeGreaterThan(0);
            stream[i - 1].Kind.Should().Be("remove",
                "抛出重授权异常之前必须已删除授权状态（一次性 refresh_token 留着只会反复撞墙）");
        }

        CountOf(code, "_store.SetAsync(").Should().Be(2,
            "只有两条成功路径可写穿存储（换码 / 刷新各一次），且必须是整对写入");
    }

    /// <summary>
    /// ADS-B6（<b>上下文登记完整性</b>）：每个源生成 <c>*JsonContext.g.cs</c> 都必须被合并进组件解析器。
    /// </summary>
    /// <remarks>
    /// 漏登记在 JIT 下一切正常（反射兜底），只在 Native AOT 的首次应答反序列化时炸，
    /// 属「编译期假绿、运行期失败」的一类 ⇒ 逐文件名钉成硬断言。
    /// 反向同理：<c>[HttpJsonSerializable]</c> 用到的每个分组名都必须有对应生成文件。
    /// </remarks>
    [Fact]
    public void AdsGeneratedJsonContexts_ShouldAllBeMergedIntoResolver()
    {
        var contextFiles = Directory
            .EnumerateFiles(
                Path.Combine(SourceProjectDir("Mud.Wechat.Ads.DataModels"), "Generated"),
                "*JsonContext.g.cs", SearchOption.AllDirectories)
            // 文件名形如 AdvertiserJsonContext.g.cs：两次去扩展才得到上下文类型名 AdvertiserJsonContext。
            .Select(static f => Path.GetFileNameWithoutExtension(Path.GetFileNameWithoutExtension(f)))
            .OrderBy(static n => n, StringComparer.Ordinal)
            .ToArray();

        contextFiles.Should().NotBeEmpty("广告线必须已有源生成 JsonContext（缺即 DTO 未标 [HttpJsonSerializable]）");

        var merger = StripComments(File.ReadAllText(AdsJsonResolverFile));
        foreach (var contextName in contextFiles)
        {
            merger.Should().Contain($"{contextName}.Default",
                $"{contextName} 未合并进组件解析器 ⇒ Native AOT 下该域 DTO 无元数据（JIT 下侥幸可用，守卫 ADS-B6）");
        }

        var groups = AdsDataModelTypes()
            .Select(static t => t.GetCustomAttribute<HttpJsonSerializableAttribute>()?.SerializerClassName)
            .Where(static n => !string.IsNullOrEmpty(n))
            .Select(static n => n!)
            .Distinct()
            .Select(static n => n + "JsonContext")
            .OrderBy(static n => n, StringComparer.Ordinal)
            .ToArray();

        groups.Should().BeEquivalentTo(contextFiles,
            "DTO 声明的分组名集合与生成文件集合必须相等：多出的分组名没有生成物、缺失的生成文件说明整域未被消费");

        // 公共基底 OAuth 与业务域同批登记（AddAdsApp 单点调用，见 AdsJsonResolverExtensions remarks）。
        merger.Should().Contain("OAuthJsonContext.Default",
            "只调 AddAdsApp、不装业务模块的宿主也要能在 AOT 下换码");
    }

    /// <summary>dynamic_creatives 域路由表断言（2026-10-11 L3 核验后建模）。</summary>
    [Fact]
    public void DynamicCreativesEndpoints_ShouldMatchOfficialRouteTable()
    {
        DynamicCreativesRoutes.Should().HaveCount(4, "官方 dynamic_creatives/* 恰 4 个端点（get/add/update/delete）");
        DynamicCreativesRoutes.Select(static r => r.Route).Distinct().Should().HaveCount(4, "路由互不重复");
        typeof(IWechatAdsDynamicCreativeService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().HaveCount(4, "接口端点数与守卫路由表条目数必须一致（新增端点未入表即红）");

        foreach (var (iface, method, httpAttribute, route) in DynamicCreativesRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");
            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约逐字符一致");
        }
    }

    /// <summary>components 域路由表断言（跨 components/* 与 component_detail/get 两支资源族）。</summary>
    [Fact]
    public void ComponentsEndpoints_ShouldMatchOfficialRouteTable()
    {
        ComponentsRoutes.Should().HaveCount(4, "官方 components/* 恰 3 个 + component_detail/get 1 个，共 4 端点");
        ComponentsRoutes.Select(static r => r.Route).Distinct().Should().HaveCount(4, "路由互不重复");
        typeof(IWechatAdsComponentService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().HaveCount(4, "接口端点数与守卫路由表条目数必须一致");

        foreach (var (iface, method, httpAttribute, route) in ComponentsRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");
            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约逐字符一致");
        }
    }

    /// <summary>
    /// 素材两域路由表断言（images/videos 各声明式 3 端点）。
    /// </summary>
    /// <remarks>
    /// <c>images/add</c> / <c>videos/add</c> 是 <c>multipart/form-data</c> 文件上传端点，
    /// <b>不在</b>声明式路由表内（手写通道承载，路径由
    /// <see cref="MaterialUploadChannels_ShouldMatchOfficialPaths"/> 锁定）——
    /// 若有人把它们建成声明式 [Post] 路由，本域反射面计数即变红。
    /// </remarks>
    [Fact]
    public void ImageAndVideoEndpoints_ShouldMatchOfficialRouteTable()
    {
        ImagesRoutes.Should().HaveCount(3, "images 的声明式面恰 3 端点（add 走手写 multipart 通道）");
        VideosRoutes.Should().HaveCount(3, "videos 的声明式面恰 3 端点（add 走手写 multipart 通道）");

        foreach (var (iface, method, httpAttribute, route) in ImagesRoutes.Concat(VideosRoutes))
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");
            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约逐字符一致");
        }
    }

    /// <summary>async_tasks 域路由表断言。</summary>
    [Fact]
    public void AsyncTasksEndpoints_ShouldMatchOfficialRouteTable()
    {
        AsyncTasksRoutes.Should().HaveCount(2, "官方 async_tasks/* 恰 2 个端点（add/get）");
        typeof(IWechatAdsAsyncTaskService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Should().HaveCount(2, "接口端点数与守卫路由表条目数必须一致");

        foreach (var (iface, method, httpAttribute, route) in AsyncTasksRoutes)
        {
            var target = iface.GetMethod(method, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            target.Should().NotBeNull($"{iface.Name}.{method} 必须存在");
            var attr = target!.GetCustomAttribute(httpAttribute) as HttpMethodAttribute;
            attr!.RequestUri.Should().Be(route, $"{iface.Name}.{method} 路由必须与官方契约逐字符一致");
        }
    }

    /// <summary>
    /// ADS-B2 / ADS-B5（新域受限面）：<c>user_token</c> 的逐页出现面 ——
    /// dynamic_creatives 的 3 支 POST 带、<c>get</c> 不带；components 仅 <c>add</c> 带
    /// （官方「特定请求参数」表逐页事实，2026-10-11 核验；其余四域各页均无）。
    /// </summary>
    [Fact]
    public void RestrictedWriteSurfaces_ShouldCarryUserToken_WhenOfficialPageListsIt()
    {
        // dynamic_creatives：三支 POST 逐一点名在场；get 显式无。
        var dcMethods = typeof(IWechatAdsDynamicCreativeService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).ToArray();
        dcMethods.Where(static m => QueryParameterNames(m).Contains("user_token"))
            .Select(static m => m.Name).OrderBy(static n => n, StringComparer.Ordinal)
            .Should().Equal(new[] { "AddAsync", "DeleteAsync", "UpdateAsync" },
                "官方 add/update/delete 三页各列 user_token，get 页无该表");
        typeof(IWechatAdsDynamicCreativeService)
            .GetMethod(nameof(IWechatAdsDynamicCreativeService.GetAsync), BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)!
            .GetParameters()
            .Select(static p => p.GetCustomAttribute<QueryAttribute>()?.Name)
            .Should().NotContain(static n => n == "user_token", "官方 get 页未列出 user_token");

        // components：仅 add；delete 与两支 get 均无。
        var compMethods = typeof(IWechatAdsComponentService)
            .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly).ToArray();
        compMethods.Where(static m => QueryParameterNames(m).Contains("user_token"))
            .Select(static m => m.Name)
            .Should().Equal(new[] { "AddAsync" }, "官方仅 components/add 页列出 user_token");

        // 素材两域与异步任务域：全部页面均未另列 user_token（反射面反证——出现即与官方不符）。
        foreach (var iface in new[] { typeof(IWechatAdsImageService), typeof(IWechatAdsVideoService), typeof(IWechatAdsAsyncTaskService) })
        {
            iface.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .SelectMany(static m => QueryParameterNames(m))
                .Should().NotContain(static n => n == "user_token",
                    $"{iface.Name} 各页官方均未列出 user_token");
        }
    }

    /// <summary>
    /// multipart 上传通道的路径与表单字段名断言（手写通道没有声明式路由，须在此锁路径）。
    /// </summary>
    /// <remarks>
    /// <b>为什么锁在这里</b>：两支上传端点不经 <c>[HttpClientApi]</c>（无路由反射面），
    /// 路径字面量在 <c>AdsMaterialUploadService</c> 私有常量里 —— 从实现类取常量断言，
    /// 常量被改写即红。<b>官方形态</b>：<c>images/add</c> 文件字段名 <c>file</c>、
    /// <c>videos/add</c> 文件字段名 <c>video_file</c>（2026-10-11 逐页核验）。
    /// </remarks>
    [Fact]
    public void MaterialUploadChannels_ShouldMatchOfficialPaths()
    {
        typeof(AdsMaterialUploadService).Assembly.Should().BeSameAs(typeof(IWechatAdsImageService).Assembly,
            "上传通道实现随主包分发（服务经 AddImagesApi/AddVideosApi 随模块装配）");

        typeof(IWechatAdsImageUploadService).Should().NotBeNull();
        typeof(IWechatAdsVideoUploadService).Should().NotBeNull();
        typeof(AdsMaterialUploadService).Should().BeAssignableTo(typeof(IWechatAdsImageUploadService))
            .And.BeAssignableTo(typeof(IWechatAdsVideoUploadService),
                "图片与视频上传共用同一实现（两个 ServiceType 各持实例）");

        // 路径常量逐字断言（私有常量经反射取值，改名即红）。
        const string imagesAddPath = "/v3.0/images/add";
        const string videosAddPath = "/v3.0/videos/add";
        GetPrivateConst(typeof(AdsMaterialUploadService), "ImagesAddPath").Should().Be(imagesAddPath);
        GetPrivateConst(typeof(AdsMaterialUploadService), "VideosAddPath").Should().Be(videosAddPath);
    }

    /// <summary>取类型私有常量的字符串值（守卫内部工具）。</summary>
    private static string GetPrivateConst(Type type, string name)
        => (string)type.GetField(name, BindingFlags.NonPublic | BindingFlags.Static)!.GetValue(null)!;

    /// <summary>advertiser 域官方路由表（3 端点，2026-10-10 逐页核验）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] AdvertiserRoutes =
    {
        (typeof(IWechatAdsAdvertiserService), nameof(IWechatAdsAdvertiserService.GetAsync),
            typeof(GetAttribute), "/v3.0/advertiser/get"),
        (typeof(IWechatAdsAdvertiserService), nameof(IWechatAdsAdvertiserService.UpdateAsync),
            typeof(PostAttribute), "/v3.0/advertiser/update"),
        (typeof(IWechatAdsAdvertiserService), nameof(IWechatAdsAdvertiserService.UpdateDailyBudgetAsync),
            typeof(PostAttribute), "/v3.0/advertiser/update_daily_budget"),
    };

    /// <summary>adgroups 域官方路由表（8 端点，2026-10-10 逐页核验；仅 get 为 GET）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] AdgroupsRoutes =
    {
        (typeof(IWechatAdsAdgroupService), nameof(IWechatAdsAdgroupService.GetAsync),
            typeof(GetAttribute), "/v3.0/adgroups/get"),
        (typeof(IWechatAdsAdgroupService), nameof(IWechatAdsAdgroupService.AddAsync),
            typeof(PostAttribute), "/v3.0/adgroups/add"),
        (typeof(IWechatAdsAdgroupService), nameof(IWechatAdsAdgroupService.UpdateAsync),
            typeof(PostAttribute), "/v3.0/adgroups/update"),
        (typeof(IWechatAdsAdgroupService), nameof(IWechatAdsAdgroupService.DeleteAsync),
            typeof(PostAttribute), "/v3.0/adgroups/delete"),
        (typeof(IWechatAdsAdgroupService), nameof(IWechatAdsAdgroupService.UpdateDailyBudgetAsync),
            typeof(PostAttribute), "/v3.0/adgroups/update_daily_budget"),
        (typeof(IWechatAdsAdgroupService), nameof(IWechatAdsAdgroupService.UpdateConfiguredStatusAsync),
            typeof(PostAttribute), "/v3.0/adgroups/update_configured_status"),
        (typeof(IWechatAdsAdgroupService), nameof(IWechatAdsAdgroupService.UpdateBidAmountAsync),
            typeof(PostAttribute), "/v3.0/adgroups/update_bid_amount"),
        (typeof(IWechatAdsAdgroupService), nameof(IWechatAdsAdgroupService.UpdateDatetimeAsync),
            typeof(PostAttribute), "/v3.0/adgroups/update_datetime"),
    };

    /// <summary>
    /// reports 域官方路由表（4 端点，2026-10-10 逐页核验；<c>async_report_files/get</c> 因基址是
    /// <c>dl.e.qq.com</c> 而未落地，见 <see cref="ReportsEndpoints_ShouldMatchOfficialRouteTable"/>）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] ReportsRoutes =
    {
        (typeof(IWechatAdsReportService), nameof(IWechatAdsReportService.GetDailyAsync),
            typeof(GetAttribute), "/v3.0/daily_reports/get"),
        (typeof(IWechatAdsReportService), nameof(IWechatAdsReportService.GetHourlyAsync),
            typeof(GetAttribute), "/v3.0/hourly_reports/get"),
        (typeof(IWechatAdsReportService), nameof(IWechatAdsReportService.AddAsyncReportAsync),
            typeof(PostAttribute), "/v3.0/async_reports/add"),
        (typeof(IWechatAdsReportService), nameof(IWechatAdsReportService.GetAsyncReportAsync),
            typeof(GetAttribute), "/v3.0/async_reports/get"),
    };

    /// <summary>dynamic_creatives 域官方路由表（4 端点，2026-10-11 L3 核验；仅 get 为 GET）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] DynamicCreativesRoutes =
    {
        (typeof(IWechatAdsDynamicCreativeService), nameof(IWechatAdsDynamicCreativeService.GetAsync),
            typeof(GetAttribute), "/v3.0/dynamic_creatives/get"),
        (typeof(IWechatAdsDynamicCreativeService), nameof(IWechatAdsDynamicCreativeService.AddAsync),
            typeof(PostAttribute), "/v3.0/dynamic_creatives/add"),
        (typeof(IWechatAdsDynamicCreativeService), nameof(IWechatAdsDynamicCreativeService.UpdateAsync),
            typeof(PostAttribute), "/v3.0/dynamic_creatives/update"),
        (typeof(IWechatAdsDynamicCreativeService), nameof(IWechatAdsDynamicCreativeService.DeleteAsync),
            typeof(PostAttribute), "/v3.0/dynamic_creatives/delete"),
    };

    /// <summary>
    /// components 域官方路由表（4 端点，2026-10-11 L3 核验；跨 <c>components/*</c> 与
    /// <c>component_detail/get</c> 两支资源族——同一模块承载，理由见 <c>AdsModule.Components</c>）。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] ComponentsRoutes =
    {
        (typeof(IWechatAdsComponentService), nameof(IWechatAdsComponentService.GetAsync),
            typeof(GetAttribute), "/v3.0/components/get"),
        (typeof(IWechatAdsComponentService), nameof(IWechatAdsComponentService.AddAsync),
            typeof(PostAttribute), "/v3.0/components/add"),
        (typeof(IWechatAdsComponentService), nameof(IWechatAdsComponentService.DeleteAsync),
            typeof(PostAttribute), "/v3.0/components/delete"),
        (typeof(IWechatAdsComponentService), nameof(IWechatAdsComponentService.GetDetailAsync),
            typeof(GetAttribute), "/v3.0/component_detail/get"),
    };

    /// <summary>
    /// images 域官方路由表（声明式 3 端点，2026-10-11 L3 核验）。
    /// <c>images/add</c> 为 <c>multipart/form-data</c> 文件上传、无声明式路由（手写通道
    /// <c>IWechatAdsImageUploadService</c>），由 <see cref="MaterialUploadChannels_ShouldMatchOfficialPaths"/> 锁路径。
    /// </summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] ImagesRoutes =
    {
        (typeof(IWechatAdsImageService), nameof(IWechatAdsImageService.GetAsync),
            typeof(GetAttribute), "/v3.0/images/get"),
        (typeof(IWechatAdsImageService), nameof(IWechatAdsImageService.UpdateAsync),
            typeof(PostAttribute), "/v3.0/images/update"),
        (typeof(IWechatAdsImageService), nameof(IWechatAdsImageService.DeleteAsync),
            typeof(PostAttribute), "/v3.0/images/delete"),
    };

    /// <summary>videos 域官方路由表（声明式 3 端点；<c>videos/add</c> 同 images/add 走手写通道）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] VideosRoutes =
    {
        (typeof(IWechatAdsVideoService), nameof(IWechatAdsVideoService.GetAsync),
            typeof(GetAttribute), "/v3.0/videos/get"),
        (typeof(IWechatAdsVideoService), nameof(IWechatAdsVideoService.UpdateAsync),
            typeof(PostAttribute), "/v3.0/videos/update"),
        (typeof(IWechatAdsVideoService), nameof(IWechatAdsVideoService.DeleteAsync),
            typeof(PostAttribute), "/v3.0/videos/delete"),
    };

    /// <summary>async_tasks 域官方路由表（2 端点，2026-10-11 L3 核验；仅 get 为 GET）。</summary>
    private static readonly (Type Interface, string Method, Type HttpAttribute, string Route)[] AsyncTasksRoutes =
    {
        (typeof(IWechatAdsAsyncTaskService), nameof(IWechatAdsAsyncTaskService.AddAsync),
            typeof(PostAttribute), "/v3.0/async_tasks/add"),
        (typeof(IWechatAdsAsyncTaskService), nameof(IWechatAdsAsyncTaskService.GetAsync),
            typeof(GetAttribute), "/v3.0/async_tasks/get"),
    };

    /// <summary>
    /// 官方 <c>daily_reports/get</c> 页 <c>level</c> 的 17 支可选值（逐字核验，写在接口 XML 上）。
    /// 三支集合<b>互不相同且差异双向</b> ⇒ 各自单列一支常量，不做公共枚举（见
    /// <see cref="ReportLevelSets_ShouldStayThreeDistinctPerPageCollections"/>）。
    /// </summary>
    private static readonly string[] DailyReportLevels =
    {
        "REPORT_LEVEL_ADVERTISER", "REPORT_LEVEL_ADGROUP", "REPORT_LEVEL_DYNAMIC_CREATIVE",
        "REPORT_LEVEL_COMPONENT", "REPORT_LEVEL_CHANNEL", "REPORT_LEVEL_BIDWORD",
        "REPORT_LEVEL_QUERYWORD", "REPORT_LEVEL_MATERIAL_IMAGE", "REPORT_LEVEL_MATERIAL_VIDEO",
        "REPORT_LEVEL_MARKETING_ASSET", "REPORT_LEVEL_PRODUCT_CATALOG", "REPORT_LEVEL_PROJECT",
        "REPORT_LEVEL_PROJECT_CREATIVE", "REPORT_LEVEL_VIDEO_HIGHLIGHT",
        "REPORT_LEVEL_PRODUCT_CREATIVE_TEMPLATE", "REPORT_LEVEL_WECHAT_SHOP_PRODUCT",
        "REPORT_LEVEL_PLAYLET",
    };

    /// <summary>官方 <c>hourly_reports/get</c> 页 <c>level</c> 的 8 支可选值（是 daily 的真子集）。</summary>
    private static readonly string[] HourlyReportLevels =
    {
        "REPORT_LEVEL_ADVERTISER", "REPORT_LEVEL_ADGROUP", "REPORT_LEVEL_DYNAMIC_CREATIVE",
        "REPORT_LEVEL_CHANNEL", "REPORT_LEVEL_BIDWORD", "REPORT_LEVEL_PROJECT",
        "REPORT_LEVEL_PROJECT_CREATIVE", "REPORT_LEVEL_VIDEO_HIGHLIGHT",
    };

    /// <summary>
    /// 官方 <c>async_reports/add</c> 页 <c>level</c> 的 21 支可选值：在 daily 集合上增人口属性等七支、
    /// 删 <c>VIDEO_HIGHLIGHT</c> / <c>WECHAT_SHOP_PRODUCT</c> / <c>PLAYLET</c> 三支。
    /// </summary>
    private static readonly string[] AsyncReportAddLevels =
    {
        "REPORT_LEVEL_ADVERTISER", "REPORT_LEVEL_ADGROUP", "REPORT_LEVEL_DYNAMIC_CREATIVE",
        "REPORT_LEVEL_COMPONENT", "REPORT_LEVEL_CHANNEL", "REPORT_LEVEL_BIDWORD",
        "REPORT_LEVEL_QUERYWORD", "REPORT_LEVEL_MATERIAL_IMAGE", "REPORT_LEVEL_MATERIAL_VIDEO",
        "REPORT_LEVEL_MARKETING_ASSET", "REPORT_LEVEL_PRODUCT_CATALOG", "REPORT_LEVEL_PROJECT",
        "REPORT_LEVEL_PROJECT_CREATIVE", "REPORT_LEVEL_PRODUCT_CREATIVE_TEMPLATE",
        "REPORT_LEVEL_AGE", "REPORT_LEVEL_GENDER", "REPORT_LEVEL_REGION", "REPORT_LEVEL_CITY",
        "REPORT_LEVEL_LANDING_PAGE", "REPORT_LEVEL_AD_UNION", "REPORT_LEVEL_OS",
    };

    /// <summary>v3.0 已移除、本 SDK 刻意不建模的资源族名（按路径段判别，见 AdsRoutes_... 守卫 remarks）。</summary>
    private static readonly HashSet<string> RemovedV3Resources = new(StringComparer.Ordinal) { "campaign", "campaigns", "ads" };

    /// <summary>
    /// 守卫各域路由表的<b>汇总面</b>：新增资源域必须同批并入（不并入即被
    /// AdsRoutes_ShouldEqualGuardTablesAndNeverReviveRemovedResources 的双向相等断言打红）。
    /// </summary>
    private static (Type Interface, string Method, Type HttpAttribute, string Route)[] AdsGuardedRoutes()
        => AdvertiserRoutes.Concat(AdgroupsRoutes).Concat(ReportsRoutes)
            .Concat(DynamicCreativesRoutes).Concat(ComponentsRoutes)
            .Concat(ImagesRoutes).Concat(VideosRoutes).Concat(AsyncTasksRoutes).ToArray();

    /// <summary>路由契约的规范化键（接口 × 方法 × 动词 × 路由），供反射面与守卫表逐条对照。</summary>
    private static string Key(string iface, string method, string verb, string route)
        => string.Join('|', iface, method, verb, route);

    /// <summary>接口反射面上的全部路由（真实形状；动词取特性类型名，与守卫表的 typeof 同源）。</summary>
    private static (string Interface, string Method, string Verb, string Route)[] AdsReflectedRoutes()
    {
        var routes = new List<(string, string, string, string)>();

        foreach (var iface in AdsBusinessInterfaces())
        {
            foreach (var method in iface.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                var verbs = method.GetCustomAttributes().OfType<HttpMethodAttribute>().ToArray();
                verbs.Should().HaveCount(1,
                    $"{iface.Name}.{method.Name} 必须恰好声明一个 HTTP 动词特性（零个 = 不被发送、多个 = 动词由特性顺序决定）");

                routes.Add((iface.Name, method.Name, verbs[0].GetType().Name, verbs[0].RequestUri!));
            }
        }

        return routes.ToArray();
    }

    /// <summary>某方法上的 <c>[Query]</c> 参数名集合（未标注者为空，不抛）。</summary>
    private static List<string> QueryParameterNames(MethodInfo method)
        => method.GetParameters()
            .Select(static p => p.GetCustomAttribute<QueryAttribute>()?.Name)
            .Where(static n => !string.IsNullOrEmpty(n))
            .Select(static n => n!)
            .ToList();

    /// <summary>
    /// 广告线全部业务接口上的 <c>[Header]</c> 参数（Owner = <c>{接口}.{方法}</c>）。
    /// v3.0 的请求头参数面极窄（已核验页面仅 <c>adgroups/add</c> 的 <c>X-Request-Id</c>）⇒
    /// 逐条带属主返回，才能同时锁住「头名」与「长在哪个方法上」。
    /// </summary>
    private static (string Owner, string Name)[] AdsHeaderParameterNames()
        => AdsBusinessInterfaces()
            .SelectMany(static i => i
                .GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
                .SelectMany(m => m.GetParameters()
                    .Select(p => (Owner: i.Name + "." + m.Name, Name: p.GetCustomAttribute<HeaderAttribute>()?.Name))))
            .Where(static x => !string.IsNullOrEmpty(x.Name))
            .Select(static x => (x.Owner, x.Name!))
            .ToArray();

    /// <summary>官方字段名的合法形状：小写字母开头，仅小写字母 / 数字 / 下划线。</summary>
    private static readonly Regex OfficialNamePattern = new("^[a-z][a-z0-9_]*$", RegexOptions.Compiled);

    private static Type TypeOf(List<(string? Name, Type Type)> declared, string name)
        => declared.Single(p => p.Name == name).Type;

    /// <summary>
    /// 从接口 XML 文本里按<b>逐页锚点</b>抽取 <c>level</c> 的官方可选值集合：
    /// 锚点后<b>第一对花括号</b>区间内匹配 <c>REPORT_LEVEL_*</c>，去重后按 Ordinal 排序。
    /// </summary>
    /// <remarks>
    /// 锚点缺失、或锚点后没有 <c>{ … }</c> 区间即<b>抛错变红</b>（不返回空集合）——
    /// 返回空集合会让上层的 <c>Equal(常量)</c> 与 <c>HaveCount(17)</c> 一起失败，但失败原因显示成
    /// 「集合不相等」而不是「文档没了」，归因成本高一个数量级。
    /// 取值集合本域刻意不建枚举，故接口文档就是唯一可机械检出的载体。
    /// </remarks>
    private static string[] ExtractReportLevels(string text, string anchor)
    {
        var anchorIndex = text.IndexOf(anchor, StringComparison.Ordinal);
        anchorIndex.Should().BeGreaterThanOrEqualTo(0,
            $"接口 XML 必须保留逐页 level 集合的锚点「{anchor}」（锚点被改写或删除即本域文档失守）");

        // char 重载只有 IndexOf(char, int)，没有 StringComparison 版本（与 string 重载不同）。
        var open = text.IndexOf('{', anchorIndex);
        var close = text.IndexOf('}', open + 1);
        (open >= 0 && close > open).Should().BeTrue(
            $"锚点「{anchor}」之后必须紧跟 <c>{{ … }}</c> 形态的官方可选值列表（找不到即列表被改成散文）");

        return Regex
            .Matches(text.Substring(open, close - open + 1), "REPORT_LEVEL_[A-Z_]+")
            .Cast<System.Text.RegularExpressions.Match>()
            .Select(static m => m.Value)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(static n => n, StringComparer.Ordinal)
            .ToArray();
    }

    /// <summary>广告线业务接口（带 <c>[HttpClientApi]</c> 且返回 Task 的接口声明面）。</summary>
    private static Type[] AdsBusinessInterfaces()
        => typeof(IWechatAdsAdvertiserService).Assembly
            .GetTypes()
            .Where(static t => t.IsInterface && t.GetCustomAttribute<HttpClientApiAttribute>() is not null)
            .ToArray();

    /// <summary>广告线 DataModels 程序集的全部类型（含 internal 生成上下文）。</summary>
    private static Type[] AdsDataModelTypes()
        => typeof(AdsResponse).Assembly.GetTypes();

    /// <summary>某类型<b>自身声明</b>的公共属性上的官方字段名（排序后返回，供逐条等值断言）。</summary>
    private static string[] OfficialJsonNames(Type type)
        => type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .Where(static n => !string.IsNullOrEmpty(n))
            .Select(static n => n!)
            .OrderBy(static n => n, StringComparer.Ordinal)
            .ToArray();

    /// <summary>按<b>官方字段名</b>取属性的 CLR 类型（层级与共用性断言用；名字不在该类型上时返回 <c>null</c>）。</summary>
    private static Type? JsonPropertyClrType(Type owner, string jsonName)
        => owner.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly)
            .FirstOrDefault(p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name == jsonName)
            ?.PropertyType;

    /// <summary>
    /// 广告线<b>报文体</b>类型：DataModels 里的 class，排除生成的 <c>*JsonContext</c>。
    /// 生成物的属性/成员命名由脚本决定、不属官方契约面，把它扫进「每个公共属性必须显式 snake_case」
    /// 会让守卫在每次重跑脚本后随机变红（且红的都是假阳性）。
    /// </summary>
    private static Type[] AdsWireDtoTypes()
        => AdsDataModelTypes()
            .Where(static t => t.IsClass && !t.Name.EndsWith("JsonContext", StringComparison.Ordinal))
            .ToArray();

    /// <summary>DataModels 全部显式声明的 JSON 属性名（去重、不排序 —— 供形状与包含性判定）。</summary>
    private static List<string> AdsJsonPropertyNames()
        => AdsWireDtoTypes()
            .SelectMany(static t => t.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            .Select(static p => p.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name)
            .Where(static n => !string.IsNullOrEmpty(n))
            .Select(static n => n!)
            .Distinct()
            .ToList();

    /// <summary>
    /// OAuth 手写传输实际上送的全部 Query 参数名（取 <c>BuildUri</c> 调用的字面量实参）。
    /// 这两支端点刻意不走声明式客户端（令牌注入分面），因此参数名只存在于源码字面量里。
    /// </summary>
    private static string[] AdsOAuthRequestParameterNames()
        => Regex
            .Matches(StripComments(File.ReadAllText(AdsAuthorizationServiceFile)), @"\(\""([a-z_]+)\"",")
            .Cast<System.Text.RegularExpressions.Match>()
            .Select(static m => m.Groups[1].Value)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(static n => n, StringComparer.Ordinal)
            .ToArray();

    /// <summary>广告线全部接口上出现的 <c>[Query]</c> 参数名（不筛凭据，供登记面反查）。</summary>
    private static List<string> AdsAllQueryParamNames()
        => typeof(IWechatAdsAdvertiserService).Assembly
            .GetTypes()
            .Where(static t => t.IsInterface)
            .SelectMany(static t => t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            .SelectMany(static m => m.GetParameters())
            .Select(static p => p.GetCustomAttribute<QueryAttribute>()?.Name)
            .Where(static n => !string.IsNullOrEmpty(n))
            .Select(static n => n!)
            .Distinct()
            .ToList();

    /// <summary>
    /// 广告线「进 Query 的凭据参数名」：接口 <c>[Query]</c> ∪ OAuth 手写 <c>BuildUri</c> 实参 ∪ 传输层注入名，
    /// 再按「名称含 token / secret / _code」筛出凭据面（与企微线 G7 同一过滤器）。
    /// </summary>
    private static string[] AdsCredentialQueryParameterNames()
    {
        var names = AdsAllQueryParamNames()
            .Concat(AdsOAuthRequestParameterNames())
            .Concat(new[] { AdsOAuthRoutes.AccessTokenParameter })
            .Where(static n => n.IndexOf("token", StringComparison.OrdinalIgnoreCase) >= 0
                               || n.IndexOf("secret", StringComparison.OrdinalIgnoreCase) >= 0
                               || n.EndsWith("_code", StringComparison.OrdinalIgnoreCase))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(static n => n, StringComparer.Ordinal)
            .ToArray();

        return names;
    }

    /// <summary>反射读取组件脱敏词表（单一事实源：<c>SensitiveUrlRedactor.SensitiveFieldNames</c>，internal 静态类）。</summary>
    private static List<string> ReadComponentSensitiveVocabulary()
    {
        var redactor = typeof(Mud.HttpUtils.ApiException).Assembly
            .GetType("Mud.HttpUtils.Helpers.SensitiveUrlRedactor");
        redactor.Should().NotBeNull("组件 Helpers.SensitiveUrlRedactor 必须存在（ADS-B5 依赖其词表）");

        var field = redactor!.GetField("SensitiveFieldNames",
            BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
        field.Should().NotBeNull("组件词表字段名变更请同步本守卫（否则 ADS-B5 恒真）");

        var value = field!.GetValue(null) as System.Collections.IEnumerable;
        value.Should().NotBeNull();

        return value!.Cast<string>().ToList();
    }

    private static bool ContainsIgnoreCase(IEnumerable<string> source, string value)
        => source.Any(n => string.Equals(n, value, StringComparison.OrdinalIgnoreCase));

    private static int CountOf(string text, string token)
    {
        var count = 0;
        var index = 0;
        while ((index = text.IndexOf(token, index, StringComparison.Ordinal)) >= 0)
        {
            count++;
            index += token.Length;
        }

        return count;
    }

    private static List<int> AllIndexesOf(string text, string token)
    {
        var indexes = new List<int>();
        var index = 0;
        while ((index = text.IndexOf(token, index, StringComparison.Ordinal)) >= 0)
        {
            indexes.Add(index);
            index += token.Length;
        }

        return indexes;
    }

    /// <summary>广告线三个源工程的 <c>.cs</c> 清单（排除 bin/obj）。</summary>
    private static string[] AdsSourceFiles()
        => new[]
        {
            SourceProjectDir("Mud.Wechat.Ads"),
            SourceProjectDir("Mud.Wechat.Ads.Abstractions"),
            SourceProjectDir("Mud.Wechat.Ads.DataModels"),
        }
            .SelectMany(dir => Directory.EnumerateFiles(dir, "*.cs", SearchOption.AllDirectories))
            .Where(static f => !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                               && !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .ToArray();

    /// <summary>
    /// <see cref="AdsQueryJson"/> 的源文件路径（编码器与 DTO 声明的字段名单源核验，ADS-B2）。
    /// 按工程 + 文件名定位而非硬编码层级，目录再迁移时守卫不随之漂移。
    /// </summary>
    private static string AdsEncoderFile => SourceSourceFile("Mud.Wechat.Ads.DataModels", "AdsQueryJson.cs");

    /// <summary>
    /// 报表域接口源文件（<c>level</c> 三支集合与逐页必填星号只存在于各端点的参数文档里 ⇒ 文本抽取的入口，
    /// 守卫 ADS-B2）。按工程 + 文件名定位，目录再迁移时守卫不随之漂移。
    /// </summary>
    private static string AdsReportInterfaceFile => SourceSourceFile("Mud.Wechat.Ads", "IWechatAdsReportService.cs");

    /// <summary><c>async_reports/add</c> 请求体 DTO 的源文件（本页 account_id 的代理商口径写在请求体字段上）。</summary>
    private static string AdsAsyncReportAddFile => SourceSourceFile("Mud.Wechat.Ads.DataModels", "AdsAsyncReportAdd.cs");

    /// <summary>授权编排实现的路径（ADS-B3 的失败顺序与刷新端点唯一性在此文本核验）。</summary>
    private static string AdsAuthorizationServiceFile => SourceSourceFile("Mud.Wechat.Ads.Abstractions", "AdsAuthorizationService.cs");

    /// <summary>AOT 上下文合并器的路径（ADS-B6 逐生成文件断言其方法体覆盖面）。</summary>
    private static string AdsJsonResolverFile => SourceSourceFile("Mud.Wechat.Ads.Abstractions", "AdsJsonResolverExtensions.cs");

    /// <summary>在某源工程目录树内按文件名定位单个源文件（排除 bin/obj，命中唯一）。</summary>
    private static string SourceSourceFile(string projectName, string fileName)
    {
        var hits = Directory
            .EnumerateFiles(SourceProjectDir(projectName), fileName, SearchOption.AllDirectories)
            .Where(static f => !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                               && !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
            .ToArray();

        hits.Should().HaveCount(1,
            $"{projectName}/{fileName} 必须存在且唯一（命中 {hits.Length} 个；同名分叉会让文本断言只读到其中一份）");

        return hits[0];
    }

    /// <summary>剔除行注释、块注释与 XML 文档注释（守卫只对**代码**计数，不得对文档提及误报）。</summary>
    private static string StripComments(string content)
        => Regex.Replace(content, @"//.*?$|/\*.*?\*/", string.Empty,
            RegexOptions.Multiline | RegexOptions.Singleline);

    /// <summary>
    /// 按 csproj 名定位源工程目录（带缓存）——源码归类到 <c>Src/&lt;Area&gt;/&lt;ProjectName&gt;</c> 后，
    /// 守卫不再硬编码层级，目录再迁移时守卫不随之漂移。
    /// </summary>
    private static string SourceProjectDir(string projectName) =>
        SourceProjectDirs.GetOrAdd(projectName, static name =>
            Directory.EnumerateFiles(Root, $"{name}.csproj", SearchOption.AllDirectories)
                .Where(static f => !f.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}", StringComparison.Ordinal)
                                   && !f.Contains($"{Path.DirectorySeparatorChar}obj{Path.DirectorySeparatorChar}", StringComparison.Ordinal))
                .Select(static f => Path.GetDirectoryName(f)!).FirstOrDefault()
            ?? throw new DirectoryNotFoundException($"未找到工程 {name}.csproj（源码归类目录漂移，守卫定位失效）"));

    private static string Root
    {
        get
        {
            var dir = new DirectoryInfo(AppContext.BaseDirectory);
            while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "Mud.Wechat.slnx")))
            {
                dir = dir.Parent;
            }

            return dir?.FullName ?? throw new InvalidOperationException("未找到仓库根（Mud.Wechat.slnx）");
        }
    }

    private static readonly System.Collections.Concurrent.ConcurrentDictionary<string, string> SourceProjectDirs = new();
}
