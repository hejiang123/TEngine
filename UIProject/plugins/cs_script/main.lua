require(PluginPath..'/xmlSimple')

local assetsPath;
local codePkgName;
local cacheXmlTable = {}
local cacheExportTable = {}
local function genCode(handler)
	cacheXmlTable = {}
	cacheExportTable = {}
	
	--主干才发布代码
	if string.len(handler.project.activeBranch) > 1 then
		return;
	end
	
    local settings = handler.project:GetSettings("Publish").codeGeneration
    codePkgName = handler:ToFilename(handler.pkg.name); --convert chinese to pinyin, remove special chars etc.
    local exportCodePath = handler.exportCodePath..'/'..codePkgName
    local namespaceName = codePkgName
    assetsPath = handler.project.assetsPath
	
    if settings.packageName~=nil and settings.packageName~='' then
        namespaceName = settings.packageName..'.'..namespaceName
    end

	handler:SetupCodeFolder(exportCodePath, "cs")
	local fileArr = CS.System.IO.Directory.GetFiles(exportCodePath, '*.*')
	for i=0, fileArr.Length-1 do
		CS.System.IO.File.Delete(fileArr[i])
	end
	
    --CollectClasses(stripeMemeber, stripeClass, fguiNamespace)
    local classes = handler:CollectClasses(false, false, nil)
    local getMemberByName = settings.getMemberByName

	--组件代码
	local conf = {}
	conf.usingTabs = 1
	conf.fileMark = '//代码自动生成，切勿修改!!!'
    local writer = CodeWriter.new(conf)
    local classCnt = classes.Count
    for i=0,classCnt-1 do
        local classInfo = classes[i]
		if classInfo.res == nil or willExportClass(classInfo.res.file) then
			local members = classInfo.members
			writer:reset()
			writer:writeln('using FairyGUI;')
			writer:writeln('using FairyGUI.Utils;')
			writer:writeln()
			writer:writeln('namespace %s', namespaceName)
			writer:startBlock()
			writer:writeln('public sealed partial class %s : %s', classInfo.className, classInfo.superClassName)
			writer:startBlock()

			local memberCnt = members.Count
			for j=0,memberCnt-1 do
				local memberInfo = members[j]
				if willMemberExport(memberInfo) then
					if memberInfo.group == 0 then
						local classType = getMemberClassName(memberInfo)
						writer:writeln('public %s %s;', classType, memberInfo.varName)
					else
						writer:writeln('public %s %s;', memberInfo.type, memberInfo.varName)
					end
				end
			end
			writer:writeln('public const string URL = "ui://%s%s";', handler.pkg.id, classInfo.resId)
			writer:writeln()

			writer:writeln('public override void ConstructFromXML(XML xml)')
			writer:startBlock()
			writer:writeln('base.ConstructFromXML(xml);')
			writer:writeln()
			for j=0,memberCnt-1 do
				local memberInfo = members[j]
				if willMemberExport(memberInfo) then
					if memberInfo.group==0 then
						local classType = getMemberClassName(memberInfo)
						if getMemberByName then
							writer:writeln('%s = (%s)GetChild("%s");', memberInfo.varName, classType, memberInfo.name)
						else
							writer:writeln('%s = (%s)GetChildAt(%s);', memberInfo.varName, classType, memberInfo.index)
						end
					elseif memberInfo.group==1 then
						if getMemberByName then
							writer:writeln('%s = GetController("%s");', memberInfo.varName, memberInfo.name)
						else
							writer:writeln('%s = GetControllerAt(%s);', memberInfo.varName, memberInfo.index)
						end
					else
						if getMemberByName then
							writer:writeln('%s = GetTransition("%s");', memberInfo.varName, memberInfo.name)
						else
							writer:writeln('%s = GetTransitionAt(%s);', memberInfo.varName, memberInfo.index)
						end
					end
				end
			end
			writer:endBlock()

			writer:endBlock() --class
			writer:endBlock() --namepsace

			writer:save(exportCodePath..'/'..classInfo.className..'.cs')
		end
    end
	
	
	--package代码
    writer:reset()
    local packageName = codePkgName..'Package'
    writer:writeln('using FairyGUI;')
    writer:writeln()
    writer:writeln('namespace %s', namespaceName)
    writer:startBlock()
    writer:writeln('public sealed class %s', packageName)
    writer:startBlock()
    writer:writeln('public const string packageId = "%s";', handler.pkg.id)
	--未导出代码的组件在package导出URL
	--for i=0,classCnt-1 do
    --    local classInfo = classes[i]
	--	if false == willExportClass(classInfo.res.file) and (classInfo.res and classInfo.res.exported) then
	--		writer:writeln('public const string URL_%s = "ui://%s%s";', classInfo.className, handler.pkg.id, classInfo.resId)
	--	end
    --end
	--writer:writeln()
	
	writer:writeln('public static GComponent GetComponent(string itemUrl)')
    writer:startBlock() --GetComponent
	writer:writeln('switch(itemUrl)')
	writer:startBlock() --switch
    for i=0,classCnt-1 do
        local classInfo = classes[i]
		if classInfo.res.branch == "" and willExportClass(classInfo.res.file) then
			writer:writeln('case %s.URL:', classInfo.className)
			writer:writeln('\treturn new %s();', classInfo.className)
		end
    end
	writer:writeln('default:')
	writer:writeln('\treturn null;')
	writer:endBlock() --switch
    writer:endBlock() --GetComponent

    writer:endBlock() --class
    writer:endBlock() --namespace
    
    writer:save(exportCodePath..'/'..packageName..'.cs')
	
	
	--helper代码
	writer:reset()
	writer:writeln('using FairyGUI;')
	writer:writeln()
	writer:writeln('public sealed class FGUIHelper')
	writer:startBlock()
	
	writer:writeln('public static string GetPackageNameByURL(string itemUrl)')
	writer:startBlock()
	writer:writeln('string pkg = itemUrl.Substring(5, 8);')
	writer:writeln('return GetPackageNameById(pkg);')
	writer:endBlock()
	
	writer:writeln()
	writer:writeln('public static string GetPackageNameById(string pkgId)')
	writer:startBlock()
	writer:writeln('switch(pkgId)')
	writer:startBlock()
	local dirArr = CS.System.IO.Directory.GetDirectories(handler.exportCodePath)
	for i=0, dirArr.Length-1 do
		local folder = CS.System.IO.Path.GetFileName(dirArr[i])
		writer:writeln('case %s.%sPackage.packageId:', folder, folder)
		writer:writeln('\treturn "%s";', folder)
	end
	writer:endBlock()
	writer:writeln('return "";')
	writer:endBlock()
	
	writer:writeln()
	writer:writeln('public static GComponent GetComponent(string itemUrl)')
	writer:startBlock()
	writer:writeln('string pkg = itemUrl.Substring(5, 8);')
	writer:writeln('switch(pkg)')
	writer:startBlock()
	local dirArr = CS.System.IO.Directory.GetDirectories(handler.exportCodePath)
	for i=0, dirArr.Length-1 do
		local folder = CS.System.IO.Path.GetFileName(dirArr[i])
		writer:writeln('case %s.%sPackage.packageId:', folder, folder)
		writer:writeln('\treturn %s.%sPackage.GetComponent(itemUrl);', folder, folder)
	end
	writer:endBlock()
	writer:writeln('return null;')
	writer:endBlock()
	
	writer:endBlock()
	writer:save(handler.exportCodePath .. '/FGUIHelper.cs')
end

--获取引用组件的包名
function getMemberClassName(memberInfo)
	if memberInfo.res == nil then
		return memberInfo.type
	end
	
	local path = memberInfo.res.file
	if willExportClass(path) then
		--获取namespace
		local nameSpace = string.gsub(path, assetsPath, '', 1)
		nameSpace = string.gsub(nameSpace, '\\', '/')
		local idx = string.find(nameSpace, '/', 2)
		nameSpace = string.sub(nameSpace, 2, idx - 1)
		if nameSpace == codePkgName then
			nameSpace = ''
		else
			nameSpace = nameSpace .. '.'
		end
		return nameSpace .. 'UI_' .. CS.System.IO.Path.GetFileNameWithoutExtension(memberInfo.res.fileName)
	end
	return getMemeberOrgType(path, memberInfo.type)
end

--字段是否会导出
function willMemberExport(memberInfo)
	local name = memberInfo.name;
	if memberInfo.type == 'Controller' then
		if false == (name == 'button' or string.find(name, 'c%d') == 1) then
			return true;
		end
	elseif memberInfo.type == 'Transition' then
		if false == (string.find(name, 't%d') == 1) then
			return true
		end
	else
		if false == (string.find(name, 'n%d') == 1) then
			return true
		end
	end
	return false;
end

--获取成员的基础类型
function getMemeberOrgType(xmlPath, defaultType)
	if false == CS.System.IO.File.Exists(xmlPath) then return defaultType end
	local xml = nil
	if cacheXmlTable[xmlPath] then
		xml = cacheXmlTable[xmlPath]
	else
		local txt = CS.System.IO.File.ReadAllText(xmlPath)
		xml = newParser():ParseXmlText(txt)
		cacheXmlTable[xmlPath] = xml;
	end
	if xml.component['@extention'] then
		return 'G' .. xml.component['@extention']
	end
	return 'GComponent'
end

--xml是否会被导出类
function willExportClass(xmlPath)
	--文件不存在
	if false == CS.System.IO.File.Exists(xmlPath) then return false end
	--分支 查找asset_xxx
	if string.find(string.gsub(xmlPath, '/', '\\'), assetsPath .. '_') then return false end
	
	local xml = nil
	if cacheXmlTable[xmlPath] then
		xml = cacheXmlTable[xmlPath]
	else
		local txt = CS.System.IO.File.ReadAllText(xmlPath)
		xml = newParser():ParseXmlText(txt)
		cacheXmlTable[xmlPath] = xml;
	end
	
	if cacheExportTable[xmlPath] ~= nil then
		return cacheExportTable[xmlPath];
	end
	
	local comp = xml.component;
	--控制器
	local ctrls = comp.controller
	if ctrls then
		if #ctrls > 0 then --多个
			for i, ctr in ipairs(ctrls) do
				if ctr['@name'] and false == (ctr['@name'] == 'button' or string.find(ctr['@name'], 'c%d') == 1) then
					cacheExportTable[xmlPath] = true
					return true
				end
			end
		else --一个
			if ctrls['@bb'] == 'fd' then
				fprint(ctrls['@name'])
				fprint(string.find(ctrls['@name'], 'c%d'));
			end
			if ctrls['@name'] and false == (ctrls['@name'] == 'button' or string.find(ctrls['@name'], 'c%d') == 1) then
				cacheExportTable[xmlPath] = true
				return true
			end
		end
	end
	
	--动画
	local trans = comp.transition
	if trans then
		if #trans > 0 then --多个
			for i, tr in ipairs(trans) do
				if tr['@name'] and false == (string.find(tr['@name'], 't%d') == 1) then
					cacheExportTable[xmlPath] = true
					return true
				end
			end
		else --一个
			if trans['@name'] and false == (string.find(trans['@name'], 't%d') == 1) then
				cacheExportTable[xmlPath] = true
				return true
			end
		end
	end
	
	--显示对象
	if comp.displayList then
		for _, item in pairs(comp.displayList) do
			if type(item) == 'table' then
				if #item > 0 then --多个
					for i, obj in ipairs(item) do
						if obj['@name'] and false == (string.find(obj['@name'], 'n%d') == 1) then
							if (obj['@name'] ~= 'title' or comp['@extention'] == nil)
							   and false == (comp['@extention'] == 'ProgressBar' and obj['@name'] == 'bar') then
								cacheExportTable[xmlPath] = true
								return true
							end
						end
					end
				else --一个
					if item['@name'] and false == (string.find(item['@name'], 'n%d') == 1) then
						if (item['@name'] ~= 'title' or comp['@extention'] == nil)
						   and false == (comp['@extention'] == 'ProgressBar' and item['@name'] == 'bar') then
							cacheExportTable[xmlPath] = true
							return true
						end
					end
				end
			end
		end
	end
	
	cacheExportTable[xmlPath] = false
	return false
end

function onPublish(handler)
    --if not handler.genCode then return end
	--始终导出脚本
    handler.genCode = false --prevent default output
	
    genCode(handler)
end

function onDestroy()
end

return genCode;