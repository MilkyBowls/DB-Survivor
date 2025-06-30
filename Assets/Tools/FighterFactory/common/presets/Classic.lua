
function apply()
	local toolBar = FF.toolBar("start")
	toolBar.iconSize = 32
	toolBar:addButton("file_new")
	toolBar:addButton("file_close") 
	toolBar:addButton("file_open") 
	toolBar:addButton("file_save") 
	toolBar:addButton("file_backup") 
	toolBar:addSeparator() 
	toolBar:addButton("tools_pal_editor") 
	toolBar:addSeparator() 
	toolBar:addButton("tools_options") 
	toolBar:addButton("file_run") 
end

iconset = "Classic"
stylesheet = "Classic"
colorset = "Classic"
