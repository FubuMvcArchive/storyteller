COMPILE_TARGET = "debug"
require "BuildUtils.rb"

BUILD_NUMBER = "0.9.0."
PRODUCT = "StoryTeller"
COPYRIGHT = 'Released under the Apache 2.0 License';
COMMON_ASSEMBLY_INFO = 'source/CommonAssemblyInfo.cs';

versionNumber = ENV["CCNetLabel"].nil? ? 0 : ENV["CCNetLabel"]



task :default => [:compile, :unit_test]

task :version do
  builder = AsmInfoBuilder.new(BUILD_NUMBER, {'Product' => PRODUCT, 'Copyright' => COPYRIGHT})
  puts "The build number is #{builder.buildnumber}"
  builder.write COMMON_ASSEMBLY_INFO  
end

task :compile => :version do
  MSBuildRunner.compile :compilemode => COMPILE_TARGET, :solutionfile => 'source/StoryTeller.sln'
end

task :unit_test => :compile do
  runner = NUnitRunner.new :compilemode => COMPILE_TARGET
  runner.executeTests ['StoryTeller.Testing']
end

task :package  do
  require 'fileutils'
  FileUtils.rm_rf 'deploy'

  Dir.mkdir 'deploy'

  FileUtils.cp 'source\StoryTellerUI\bin\Debug\ICSharpCode.AvalonEdit.dll', 'deploy'
  FileUtils.cp 'source\StoryTellerUI\bin\Debug\ICSharpCode.Core.dll', 'deploy'
  FileUtils.cp 'source\StoryTellerUI\bin\Debug\ICSharpCode.Core.Presentation.dll', 'deploy'
  FileUtils.cp 'source\StoryTellerUI\bin\Debug\ICSharpCode.Core.WinForms.dll', 'deploy'
  FileUtils.cp 'source\StoryTellerUI\bin\Debug\ICSharpCode.SharpDevelop.dll', 'deploy'
  FileUtils.cp 'source\StoryTellerUI\bin\Debug\ICSharpCode.TextEditor.dll', 'deploy'
  FileUtils.cp 'source\StoryTellerUI\bin\Debug\StoryTeller.UserInterface.dll', 'deploy'
  FileUtils.cp 'source\StoryTellerUI\bin\Debug\StoryTeller.UserInterface.pdb', 'deploy'
  FileUtils.cp 'source\StoryTellerUI\bin\Debug\XmlEditor.dll', 'deploy'
  FileUtils.cp 'source\StoryTellerUI\bin\Debug\StoryTeller.dll', 'deploy'
  FileUtils.cp 'source\StoryTellerUI\bin\Debug\StoryTeller.pdb', 'deploy'
  FileUtils.cp 'source\StoryTellerUI\bin\Debug\StoryTellerUI.exe', 'deploy'
  FileUtils.cp 'source\StoryTellerUI\bin\Debug\StructureMap.dll', 'deploy'
end



