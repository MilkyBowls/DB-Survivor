
function apply()
	local toolBar = FF.toolBar("start")
	toolBar.iconSize = 32
	toolBar:addButton("file_new")
	toolBar:addButton("file_close") 
	toolBar:addButton("file_open") 
	toolBar:addButton("file_save") 
	toolBar:addSeparator() 
	toolBar:addButton("file_run") 
	toolBar:addSeparator() 
	toolBar:addButton("tools_options") 
end

iconset = "Nuvola"
stylesheet = "Dark"
colorset = "Dark"
