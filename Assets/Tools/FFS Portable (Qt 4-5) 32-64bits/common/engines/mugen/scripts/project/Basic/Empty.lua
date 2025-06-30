
local dlg = FF.wizard()
dlg.windowTitle = FF.translate("Empty project")

local page = FF.wizardPage()
page.title = dlg.windowTitle
page.subTitle = FF.translate("Choose the type of the new blank project")
dlg:addPage(page)

local char = FF.radioButton(FF.translate("Character"), page)
local stage = FF.radioButton(FF.translate("Stage"), page)
local font = FF.radioButton(FF.translate("Font"), page)
local story = FF.radioButton(FF.translate("Storyboard"), page)

local lay = FF.verticalLayout(page)
lay:addWidget(char)
lay:addWidget(stage)
lay:addWidget(font)
lay:addWidget(story)

char.checked = true

if dlg:exec() > 0 then

	local projectType = MUGEN.ProjectType.Storyboard
	if char.checked then
	  projectType = MUGEN.ProjectType.Character
	elseif stage.checked then
	  projectType = MUGEN.ProjectType.Stage
	elseif font.checked then
	  projectType = MUGEN.ProjectType.Font
	end

	return FF.emptyProject("mugen", projectType)
else
	return nil
end
